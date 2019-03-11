﻿using System;
using System.ComponentModel;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <remarks>
/// 枚举类
/// </remarks> 
namespace Aster.Common.Utils
{
	/// <summary>
	/// 把枚举值按照指定的文本显示
	/// <remarks>
	/// 一般通过枚举值的ToString()可以得到变量的文本，
	/// 但是有时候需要的到与之对应的更充分的文本，
	/// 这个类帮助达到此目的
	/// </remarks>
	/// </summary>
	/// <example>
	/// [EnumDescription("中文数字")]
	/// enum MyEnum
	/// {
	///		[EnumDescription("数字一")]
	/// 	One = 1, 
	/// 
	///		[EnumDescription("数字二")]
	///		Two, 
	/// 
	///		[EnumDescription("数字三")]
	///		Three
	/// }
	/// EnumDescription.GetEnumText(typeof(MyEnum));
	/// EnumDescription.GetFieldText(MyEnum.Two);
	/// EnumDescription.GetFieldTexts(typeof(MyEnum)); 
	/// </example>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Enum)]
	public class EnumDescription : Attribute
	{
		private string enumDisplayText = string.Empty;
		private int enumRank;
		private FieldInfo fieldInfo = null;

		/// <summary>
		/// 描述枚举值
		/// </summary>
		/// <param name="enumDisplayText">描述内容</param>
		/// <param name="enumRank">排列顺序</param>
		public EnumDescription(string enumDisplayText, int enumRank)
		{
			this.enumDisplayText = enumDisplayText;
			this.enumRank = enumRank;
		}

		/// <summary>
		/// 描述枚举值，默认排序为5
		/// </summary>
		/// <param name="enumDisplayText">描述内容</param>
		public EnumDescription(string enumDisplayText) : this(enumDisplayText, 5) { }

		public string EnumDisplayText
		{
			get { return this.enumDisplayText; }
		}

		public int EnumRank
		{
			get { return enumRank; }
		}

		public int EnumValue
		{
			get { return (int)fieldInfo.GetValue(null); }
		}

		public string FieldName
		{
			get { return fieldInfo.Name; }
		}

		#region  对枚举描述属性的解释相关函数

		/// <summary>
		/// 排序类型
		/// </summary>
		public enum SortType
		{
			/// <summary>
			///按枚举顺序默认排序
			/// </summary>
			Default,
			/// <summary>
			/// 按描述值排序
			/// </summary>
			DisplayText,
			/// <summary>
			/// 按排序值排序
			/// </summary>
			Rank
		}

        private static System.Collections.Hashtable cachedEnum = new Hashtable();

		/// <summary>
		/// 得到对枚举的描述文本
		/// </summary>
		/// <param name="enumType">枚举类型</param>
		/// <returns></returns>
		public static string GetEnumText(Type enumType)
		{
			EnumDescription[] eds = (EnumDescription[])enumType.GetCustomAttributes(typeof(EnumDescription), false);
			if (eds.Length != 1) return string.Empty;
			return eds[0].EnumDisplayText;
		}

		/// <summary>
		/// 获得指定枚举类型中，指定值的描述文本。
		/// </summary>
		/// <param name="enumValue">枚举值，不要作任何类型转换</param>
		/// <returns>描述字符串</returns>
		public static string GetFieldText(object enumValue)
		{
			EnumDescription[] descriptions = GetFieldTexts(enumValue.GetType(), SortType.Default);
			foreach (EnumDescription ed in descriptions)
			{
				if (ed.fieldInfo.Name == enumValue.ToString()) return ed.EnumDisplayText;
			}
			return string.Empty;
		}

		/// <summary>
		/// 获取指定枚举类型中，指定值的数值
		/// </summary>
		/// <param name="enumValue"></param>
		/// <returns></returns>
		public static int GetFieldValue(object enumValue)
		{
			EnumDescription[] descriptions = GetFieldTexts(enumValue.GetType(), SortType.Default);
			foreach (EnumDescription ed in descriptions)
			{
				if (ed.FieldName == enumValue.ToString()) return ed.EnumValue;
			}
			return 0;
		}

		/// <summary>
		/// 根据数值获取名称
		/// </summary>
		/// <param name="type"></param>
		/// <param name="enumValue"></param>
		/// <returns></returns>
		public static string GetFieldName(Type type, int enumValue)
		{
			EnumDescription[] descriptions = GetFieldTexts(type, SortType.Default);
			foreach (EnumDescription ed in descriptions)
			{
				if (ed.EnumValue == enumValue) return ed.FieldName;
			}
			return string.Empty;
		}

		/// <summary>
		/// 根据数值获取描述
		/// </summary>
		/// <param name="type"></param>
		/// <param name="enumValue"></param>
		/// <returns></returns>
		public static string GetFieldText(Type type, int enumValue)
		{
			EnumDescription[] descriptions = GetFieldTexts(type, SortType.Default);
			foreach (EnumDescription ed in descriptions)
			{
				if (ed.EnumValue == enumValue) return ed.EnumDisplayText;
			}
			return "未知";
		}

		/// <summary>
		/// 根据名称获取描述
		/// </summary>
		/// <param name="type"></param>
		/// <param name="enumValue"></param>
		/// <returns></returns>
		public static string GetFieldText(Type type, string enumValue)
		{
			EnumDescription[] descriptions = GetFieldTexts(type, SortType.Default);
			foreach (EnumDescription ed in descriptions)
			{
				if (ed.fieldInfo.Name == enumValue.ToString()) return ed.EnumDisplayText;
			}
			return string.Empty;
		}

		/// <summary>
		/// 得到枚举类型定义的所有文本，按定义的顺序返回
		/// </summary>
		/// <exception cref="NotSupportedException"></exception>
		/// <param name="enumType">枚举类型</param>
		/// <returns>所有定义的文本</returns>
		public static EnumDescription[] GetFieldTexts(Type enumType)
		{
			return GetFieldTexts(enumType, SortType.Default);
		}

		/// <summary>
		/// 得到枚举类型定义的所有文本
		/// </summary>
		/// <exception cref="NotSupportedException"></exception>
		/// <param name="enumType">枚举类型</param>
		/// <param name="sortType">指定排序类型</param>
		/// <returns>所有定义的文本</returns>
		public static EnumDescription[] GetFieldTexts(Type enumType, SortType sortType)
		{
			EnumDescription[] descriptions = null;
            //缓存中没有找到，通过反射获得字段的描述信息
            lock (cachedEnum.SyncRoot)
            {
                if (cachedEnum.Contains(enumType.FullName) == false)
                {
                    FieldInfo[] fields = enumType.GetFields();
                    ArrayList edAL = new ArrayList();
                    foreach (FieldInfo fi in fields)
                    {
                        object[] eds = fi.GetCustomAttributes(typeof(EnumDescription), false);
                        if (eds.Length != 1) continue;
                        ((EnumDescription)eds[0]).fieldInfo = fi;
                        edAL.Add(eds[0]);
                    }
                    cachedEnum.Add(enumType.FullName, (EnumDescription[])edAL.ToArray(typeof(EnumDescription)));

                }
            }
			descriptions = (EnumDescription[])cachedEnum[enumType.FullName];
			//if (descriptions.Length <= 0) throw new NotSupportedException("枚举类型[" + enumType.Name + "]未定义属性EnumValueDescription");
			//按指定的属性冒泡排序
			for (int m = 0; m < descriptions.Length; m++)
			{
				//默认就不排序了
				if (sortType == SortType.Default) break;
				for (int n = m; n < descriptions.Length; n++)
				{
					EnumDescription temp;
					bool swap = false;
					switch (sortType)
					{
						case SortType.Default:
							break;
						case SortType.DisplayText:
							if (string.Compare(descriptions[m].EnumDisplayText, descriptions[n].EnumDisplayText) > 0) swap = true;
							break;
						case SortType.Rank:
							if (descriptions[m].EnumRank > descriptions[n].EnumRank) swap = true;
							break;
					}
					if (swap)
					{
						temp = descriptions[m];
						descriptions[m] = descriptions[n];
						descriptions[n] = temp;
					}
				}
			}
			return descriptions;
		}

		// <summary>
		/// 获取枚举值上的Description特性的说明
		/// </summary>
		/// <typeparam name="T">枚举类型</typeparam>
		/// <param name="obj">枚举值</param>
		/// <returns>特性的说明</returns>
		public static string GetEnumDescription<T>(T obj)
		{
			var type = obj.GetType();
			FieldInfo field = type.GetField(Enum.GetName(type, obj));
			if (field != null)
			{
				DescriptionAttribute descAttr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
				if (descAttr == null)
				{
					return string.Empty;
				}

				return descAttr.Description;
			}
			return string.Empty;
		}
		#endregion

	}



	public static class EnumExtend
	{
		/// <summary>
		/// 获取所有枚举的集合
		/// </summary>
		/// <param name="enumName">the type of enumName</param>
		/// <returns></returns>
		public static List<dynamic> GetAllItems(this Type enumName)
		{
			List<dynamic> list = new List<dynamic>();
			// get enum fileds
			FieldInfo[] fields = enumName.GetFields();
			foreach (FieldInfo field in fields)
			{
				if (!field.FieldType.IsEnum)
				{
					continue;
				}
				// get enum value
				int value = (int)enumName.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
				string text = field.Name;
				string description = string.Empty;
				object[] array = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (array.Length > 0)
				{
					description = ((DescriptionAttribute)array[0]).Description;
				}
				else
				{
					description = ""; //none description,set empty
				}
				//add to list
				//dynamic obj = new ExtObject();
				//obj.Value = value;
				//obj.Text = text;
				//obj.Description = description;

				var objInfo = new
				{
					Value=value,
					Text=text,
					Description = description
				};
				list.Add(objInfo);
			}
			return list;
		}


		/// <summary>
		/// 获取指定枚举的集合列表
		/// </summary>
		/// <param name="enumName"></param>
		/// <returns></returns>
		public static List<ExtObject> GetAllItemList(this Type enumName)
		{
			List<ExtObject> list = new List<ExtObject>();
			// get enum fileds
			FieldInfo[] fields = enumName.GetFields();
			foreach (FieldInfo field in fields)
			{
				if (!field.FieldType.IsEnum)
				{
					continue;
				}
				// get enum value
				int value = (int)enumName.InvokeMember(field.Name, BindingFlags.GetField, null, null, null);
				string text = field.Name;
				string description = string.Empty;
				object[] array = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (array.Length > 0)
				{
					description = ((DescriptionAttribute)array[0]).Description;
				}
				else
				{
					description = ""; //none description,set empty
				}
				//add to list
				dynamic obj = new ExtObject();
				obj.Value = value;
				obj.Text = text;
				obj.Description = description;			
				list.Add(obj);
			}
			return list;
		}

		/// <summary>
		/// 枚举对象
		/// </summary>
		public class ExtObject
		{
			public int Value { get; set; }
			public string Text { get; set; }
			public string Description { get; set; }
		}
	}	
}
