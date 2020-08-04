using Blazui.Component.ControlConfigs;
using Blazui.Component.ControlRender;
using Blazui.Component.ControlRenders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Blazui.Component
{
    public class TableEditorMap
    {
        private IDictionary<Func<PropertyInfo, EditorAttribute, bool>, Type> propertyEditorMap = new Dictionary<Func<PropertyInfo, EditorAttribute, bool>, Type>();
        private IDictionary<Func<PropertyInfo, Type, bool>, Type> editorRenderMap = new Dictionary<Func<PropertyInfo, Type, bool>, Type>();
        public TableEditorMap()
        {
            InitilizePropertyEditorMap();

            editorRenderMap.Add((property, control) => control == typeof(BDatePicker), typeof(IDatePickerRender));
            editorRenderMap.Add((property, control) => control.IsGenericType && control.GetGenericTypeDefinition() == typeof(BInput<>), typeof(IInputRender));
            editorRenderMap.Add((property, control) => control == typeof(BSwitch<bool>), typeof(ISwitchRender));
            editorRenderMap.Add((property, control) => control.IsGenericType && control.GetGenericTypeDefinition() == typeof(BSelect<>), typeof(ISelectRender));
            editorRenderMap.Add((property, control) => control == typeof(BSwitch<bool?>), typeof(ISwitchRender));

        }

        private void InitilizePropertyEditorMap()
        {
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(BSelect<string>), typeof(BSelect<string>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(BSelect<int>), typeof(BSelect<int>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(BSelect<int?>), typeof(BSelect<int?>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(BSelect<float>), typeof(BSelect<float>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(BSelect<short>), typeof(BSelect<short>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(BSelect<long>), typeof(BSelect<long>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(BSelect<DateTime>), typeof(BSelect<DateTime>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(BSelect<float?>), typeof(BSelect<float>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(BSelect<short?>), typeof(BSelect<short>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(BSelect<long?>), typeof(BSelect<long>));
            propertyEditorMap.Add((property, editorAttribute) => editorAttribute != null && editorAttribute.Control == typeof(BSelect<DateTime?>), typeof(BSelect<DateTime?>));
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
            }, typeof(BTable));
            propertyEditorMap.Add((property, editorAttribute) =>
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
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(IFileModel[]), typeof(BUpload));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(string), typeof(BInput<string>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(int), typeof(BInput<int>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(int?), typeof(BInput<int?>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(DateTime), typeof(BDatePicker));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(DateTime?), typeof(BDatePicker));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(decimal), typeof(BInput<decimal>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(decimal?), typeof(BInput<decimal?>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(float), typeof(BInput<float>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(float?), typeof(BInput<float?>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(double?), typeof(BInput<double?>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(double), typeof(BInput<double>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(bool), typeof(BSwitch<bool>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(bool?), typeof(BSwitch<bool?>));
            propertyEditorMap.Add((property, editorAttribute) => property.PropertyType == typeof(List<string>), typeof(BSelect<string>));
        }

        internal (Type ControlType, Type RenderType, Type DataSourceLoader) GetControl(PropertyInfo propertyInfo, PropertyInfo entityProperty)
        {
            var editorAttribute = propertyInfo.GetCustomAttribute<EditorAttribute>();
            var control = propertyEditorMap.FirstOrDefault(x => x.Key(entityProperty ?? propertyInfo, editorAttribute)).Value;
            var renderType = editorRenderMap.FirstOrDefault(x => x.Key(entityProperty ?? propertyInfo, control)).Value;
            if (renderType == null)
            {
                throw new BlazuiException($"属性 {propertyInfo.Name} 类型为 {propertyInfo.PropertyType} 对应的渲染器不存在");
            }
            Type dataSourceLoader = null;
            if (control.IsGenericType)
            {
                if (control.GetGenericTypeDefinition() == typeof(BSelect<>))
                {
                    dataSourceLoader = propertyInfo.GetCustomAttribute<SelectAttribute>()?.DataSourceLoader;
                }
            }
            return (control, renderType, dataSourceLoader);
        }
    }
}
