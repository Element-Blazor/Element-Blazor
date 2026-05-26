using Element.ControlConfigs;
using Element.ControlRender;
using Element.ControlRenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Element
{
    public class TableEditorMap
    {
        private IDictionary<Func<PropertyInfo, EditorGeneratorAttribute, bool>, Type> propertyEditorMap = new Dictionary<Func<PropertyInfo, EditorGeneratorAttribute, bool>, Type>();
        private IDictionary<Func<PropertyInfo, Type, bool>, Type> editorRenderMap = new Dictionary<Func<PropertyInfo, Type, bool>, Type>();
        public TableEditorMap()
        {
            InitilizePropertyEditorMap();

            editorRenderMap.Add((property, control) => control == typeof(ElDatePicker), typeof(IDatePickerRender));
            editorRenderMap.Add((property, control) => control.IsGenericType && control.GetGenericTypeDefinition() == typeof(ElInput<>), typeof(IInputRender));
            editorRenderMap.Add((property, control) => control == typeof(ElSwitch<bool>), typeof(ISwitchRender));
            editorRenderMap.Add((property, control) => control.IsGenericType && control.GetGenericTypeDefinition() == typeof(ElSelect<>), typeof(ISelectRender));
            editorRenderMap.Add((property, control) => control == typeof(ElSwitch<bool?>), typeof(ISwitchRender));

        }

        private void InitilizePropertyEditorMap()
        {
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(ElSelect<string>), typeof(ElSelect<string>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(ElSelect<int>), typeof(ElSelect<int>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(ElSelect<int?>), typeof(ElSelect<int?>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(ElSelect<float>), typeof(ElSelect<float>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(ElSelect<short>), typeof(ElSelect<short>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(ElSelect<long>), typeof(ElSelect<long>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(ElSelect<DateTime>), typeof(ElSelect<DateTime>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(ElSelect<float?>), typeof(ElSelect<float>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(ElSelect<short?>), typeof(ElSelect<short>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(ElSelect<long?>), typeof(ElSelect<long>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(ElSelect<DateTime?>), typeof(ElSelect<DateTime?>));
            propertyEditorMap.Add((property, editorAttribute) =>
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
            }, typeof(ElTable));
            propertyEditorMap.Add((property, editorAttribute) =>
            {
                if (property.PropertyType.IsEnum)
                {
                    return true;
                }
                if (!property.PropertyType.IsGenericType)
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
            }, typeof(ElSelect<>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(IFileModel[]), typeof(ElUpload));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(string), typeof(ElInput<string>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(int), typeof(ElInput<int>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(int?), typeof(ElInput<int?>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(DateTime), typeof(ElDatePicker));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(DateTime?), typeof(ElDatePicker));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(decimal), typeof(ElInput<decimal>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(decimal?), typeof(ElInput<decimal?>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(float), typeof(ElInput<float>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(float?), typeof(ElInput<float?>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(double?), typeof(ElInput<double?>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(double), typeof(ElInput<double>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(bool), typeof(ElSwitch<bool>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(bool?), typeof(ElSwitch<bool?>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(List<string>), typeof(ElSelect<string>));
        }

        internal (Type ControlType, Type RenderType, Type DataSourceLoader) GetControl(PropertyInfo propertyInfo)
        {
            var editorAttribute = propertyInfo.GetCustomAttribute<EditorGeneratorAttribute>();
            var control = propertyEditorMap.FirstOrDefault(x => x.Key(propertyInfo, editorAttribute)).Value;
            var renderType = editorRenderMap.FirstOrDefault(x => x.Key(propertyInfo, control)).Value;
            if (renderType == null)
            {
                throw new ElementException($"属性 {propertyInfo.Name} 类型为 {propertyInfo.PropertyType} 对应的渲染器不存在");
            }
            Type dataSourceLoader = null;
            if (control.IsGenericType)
            {
                if (control.GetGenericTypeDefinition() == typeof(ElSelect<>))
                {
                    dataSourceLoader = propertyInfo.GetCustomAttribute<SelectAttribute>()?.DataSourceLoader;
                }
            }
            return (control, renderType, dataSourceLoader);
        }
    }
}
