using Element.ControlConfigs;
using Element.ControlRender;
using Element.ControlRenders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Element
{
    public class FormFieldControlMap
    {
        private Dictionary<Func<PropertyInfo, bool>, Type> fieldsControlMap = new Dictionary<Func<PropertyInfo, bool>, Type>();
        private Dictionary<string, List<BFormItemObject>> formControls = new Dictionary<string, List<BFormItemObject>>();
        public FormFieldControlMap(IServiceProvider provider)
        {
            fieldsControlMap.Add(property => property.PropertyType == typeof(string), typeof(BInput<string>));
            fieldsControlMap.Add(property => property.PropertyType == typeof(int), typeof(BInput<int>));
            fieldsControlMap.Add(property => property.PropertyType == typeof(int?), typeof(BInput<int?>));
            fieldsControlMap.Add(property => property.PropertyType == typeof(DateTime), typeof(BDatePicker));
            fieldsControlMap.Add(property => property.PropertyType == typeof(DateTime?), typeof(BDatePicker));
            fieldsControlMap.Add(property => property.PropertyType == typeof(decimal), typeof(BInput<decimal>));
            fieldsControlMap.Add(property => property.PropertyType == typeof(decimal?), typeof(BInput<decimal?>));
            fieldsControlMap.Add(property => property.PropertyType == typeof(float), typeof(BInput<float>));
            fieldsControlMap.Add(property => property.PropertyType == typeof(float?), typeof(BInput<float?>));
            fieldsControlMap.Add(property => property.PropertyType == typeof(double?), typeof(BInput<double?>));
            fieldsControlMap.Add(property => property.PropertyType == typeof(double), typeof(BInput<double>));
            fieldsControlMap.Add(property => property.PropertyType == typeof(bool), typeof(BSwitch<bool>));
            fieldsControlMap.Add(property => property.PropertyType == typeof(bool?), typeof(BSwitch<bool?>));
            fieldsControlMap.Add(property => property.PropertyType == typeof(List<string>), typeof(BSelect<string>));
            fieldsControlMap.Add(property =>
            {
                if (property.PropertyType == typeof(IDictionary<string, string>))
                {
                    return true;
                }
                if (property.PropertyType == typeof(Dictionary<string, string>))
                {
                    return true;
                }
                return false;
            }, typeof(BTable));
            fieldsControlMap.Add(property =>
            {
                if (property.PropertyType.IsEnum)
                {
                    return true;
                }
                if (!property.PropertyType.IsGenericParameter)
                {
                    return false;
                }
                if (property.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
                {
                    return false;
                }
                if (Nullable.GetUnderlyingType(property.PropertyType).IsEnum)
                {
                    return true;
                }
                return false;
            }, typeof(BSelect<>));
            fieldsControlMap.Add(property => property.PropertyType == typeof(IFileModel[]), typeof(BUpload));
            this.provider = provider.CreateScope().ServiceProvider;
        }

        private IServiceProvider provider { get; }

        internal List<FormItemConfig> GetFormItems(Type entityType)
        {
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            var formItems = new List<FormItemConfig>();
            var index = 0;
            foreach (var property in properties)
            {
                Type controlType = null;
                foreach (var map in fieldsControlMap)
                {
                    if (!map.Key(property))
                    {
                        continue;
                    }
                    controlType = map.Value;
                }
                if (controlType == null)
                {
                    throw new ElementException($"类型 {property.PropertyType.FullName} 没有配置对应的组件，表单无法生成");
                }
                var formControl = property.GetCustomAttribute<FormControlAttribute>() ?? new FormControlAttribute()
                {
                    LabelWidth = 100
                };
                if (formControl.LabelWidth <= 0)
                {
                    formControl.LabelWidth = 100;
                }
                var editorGeneratorAttr = property.GetCustomAttribute<EditorGeneratorAttribute>();
                var formItemType = typeof(BFormItem<>).MakeGenericType(property.PropertyType);
                if (controlType.IsGenericType && controlType.GetGenericTypeDefinition() == typeof(BSelect<>))
                {
                    controlType = controlType.MakeGenericType(property.PropertyType);
                }

                formItems.Add(new FormItemConfig()
                {
                    SortNo = formControl.SortNo,
                    FormItem = formItemType,
                    IsRequired = editorGeneratorAttr?.IsRequired ?? true,
                    RequiredMessage = editorGeneratorAttr.RequiredMessage ?? "请填写此字段",
                    InputControlType = controlType,
                    Ignore = editorGeneratorAttr?.Ignore ?? false,
                    InputControlRender = GetInputControlRender(controlType),
                    Label = editorGeneratorAttr?.Label ?? property.Name,
                    Image = editorGeneratorAttr.Image,
                    LabelWidth = formControl.LabelWidth,
                    Placeholder = editorGeneratorAttr.Placeholder,
                    Name = property.Name,
                    Property = property,
                    ControlAttribute = GetInputControlConfig(property, controlType)
                });
                index += 11;
            }
            return formItems.Any(x => x.SortNo > 0) ? formItems.OrderBy(x => x.SortNo).ToList() : formItems;
        }


        private object GetInputControlConfig(PropertyInfo propertyInfo, Type controlType)
        {
            if (propertyInfo.PropertyType == typeof(IFileModel[]))
            {
                var uploadAttr = propertyInfo.GetCustomAttribute<UploadAttribute>();
                if (uploadAttr == null)
                {
                    throw new ElementException("IFileModel[] 类型的属性必须标记 UploadAttribute 特性");
                }
                return uploadAttr;
            }
            if (!propertyInfo.PropertyType.IsGenericType)
            {
                if (controlType.GetGenericTypeDefinition() == typeof(BInput<>))
                {
                    return propertyInfo.GetCustomAttribute<InputAttribute>();
                }
                if (controlType.GetGenericTypeDefinition() == typeof(BCheckBox<>))
                {
                    return propertyInfo.GetCustomAttribute<CheckBoxAttribute>() ?? throw new ElementException($"复选框组件所对应的属性必须标记 {nameof(CheckBoxAttribute)} 特性");
                }
            }
            if (propertyInfo.PropertyType == typeof(IDictionary<string, string>)
                || propertyInfo.PropertyType == typeof(Dictionary<string, string>))
            {
                return propertyInfo.GetCustomAttribute<TableAttribute>();
            }
            return null;
        }

        private IControlRender GetInputControlRender(Type controlType)
        {
            if (controlType == typeof(BTable))
            {
                return this.provider.GetRequiredService<ITableRender>();
            }
            if (controlType == typeof(BDatePicker))
            {
                return this.provider.GetRequiredService<IDatePickerRender>();
            }
            if (controlType == typeof(BUpload))
            {
                return this.provider.GetRequiredService<IUploadRender>();
            }
            if (!controlType.IsGenericType)
            {
                if (controlType == typeof(BCheckBox<>))
                {
                    return this.provider.GetRequiredService<ICheckBoxRender>();
                }
                throw new ElementException($"组件 {controlType.FullName} 尚未实现对应的渲染器");
            }
            var genericDefine = controlType.GetGenericTypeDefinition();
            if (genericDefine == typeof(BInput<>))
            {
                return this.provider.GetRequiredService<IInputRender>();
            }

            if (genericDefine == typeof(BCheckBox<>))
            {
                return this.provider.GetRequiredService<ICheckBoxRender>();
            }

            if (genericDefine == typeof(BSwitch<>))
            {
                return this.provider.GetRequiredService<ISwitchRender>();
            }
            if (genericDefine.GetGenericTypeDefinition() == typeof(BSelect<>))
            {
                return this.provider.GetRequiredService<ISelectRender>();
            }
            throw new ElementException($"组件 {controlType.FullName} 尚未实现对应的渲染器");
        }
    }
}
