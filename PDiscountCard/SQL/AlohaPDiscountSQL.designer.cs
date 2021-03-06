#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PDiscountCard.SQL
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="AlohaPDiscount")]
	public partial class AlohaPDiscountSQLDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertOrderTime(OrderTime instance);
    partial void UpdateOrderTime(OrderTime instance);
    partial void DeleteOrderTime(OrderTime instance);
    #endregion
		
		public AlohaPDiscountSQLDataContext() : 
				base(global::PDiscountCard.Properties.Settings.Default.AlohaPDiscountConnectionString1, mappingSource)
		{
			OnCreated();
		}
		
		public AlohaPDiscountSQLDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AlohaPDiscountSQLDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AlohaPDiscountSQLDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AlohaPDiscountSQLDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<WestITM> WestITMs
		{
			get
			{
				return this.GetTable<WestITM>();
			}
		}
		
		public System.Data.Linq.Table<WestHEADER> WestHEADERs
		{
			get
			{
				return this.GetTable<WestHEADER>();
			}
		}
		
		public System.Data.Linq.Table<OrderTime> OrderTimes
		{
			get
			{
				return this.GetTable<OrderTime>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.WestITM")]
	public partial class WestITM
	{
		
		private System.Nullable<int> _Barcode;
		
		private System.Nullable<int> _Quantity;
		
		public WestITM()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Barcode", DbType="Int")]
		public System.Nullable<int> Barcode
		{
			get
			{
				return this._Barcode;
			}
			set
			{
				if ((this._Barcode != value))
				{
					this._Barcode = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Quantity", DbType="Int")]
		public System.Nullable<int> Quantity
		{
			get
			{
				return this._Quantity;
			}
			set
			{
				if ((this._Quantity != value))
				{
					this._Quantity = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.WestHEADER")]
	public partial class WestHEADER
	{
		
		private string _Name;
		
		private System.Nullable<System.DateTime> _DateStart;
		
		private System.Nullable<System.DateTime> _DateEnd;
		
		private System.Nullable<int> _Plan;
		
		public WestHEADER()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="Char(50)")]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this._Name = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DateStart", DbType="DateTime")]
		public System.Nullable<System.DateTime> DateStart
		{
			get
			{
				return this._DateStart;
			}
			set
			{
				if ((this._DateStart != value))
				{
					this._DateStart = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DateEnd", DbType="DateTime")]
		public System.Nullable<System.DateTime> DateEnd
		{
			get
			{
				return this._DateEnd;
			}
			set
			{
				if ((this._DateEnd != value))
				{
					this._DateEnd = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[Plan]", Storage="_Plan", DbType="Int")]
		public System.Nullable<int> Plan
		{
			get
			{
				return this._Plan;
			}
			set
			{
				if ((this._Plan != value))
				{
					this._Plan = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.OrderTime")]
	public partial class OrderTime : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private System.Nullable<int> _CheckId;
		
		private System.Nullable<System.DateTime> _BusinessDate;
		
		private System.Nullable<System.DateTime> _SystemDate;
		
		private System.Nullable<int> _BarCode;
		
		private System.Nullable<int> _TableId;
		
		private System.Nullable<int> _QueueId;
		
		private System.Nullable<int> _EmployeeId;
		
		private System.Nullable<int> _ModeId;
		
		private System.Nullable<int> _EntryId;
		
		private System.Nullable<bool> _Sale;
		
		private System.Nullable<int> _EmployeeOwner;
		
		private bool _DishClosed;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnCheckIdChanging(System.Nullable<int> value);
    partial void OnCheckIdChanged();
    partial void OnBusinessDateChanging(System.Nullable<System.DateTime> value);
    partial void OnBusinessDateChanged();
    partial void OnSystemDateChanging(System.Nullable<System.DateTime> value);
    partial void OnSystemDateChanged();
    partial void OnBarCodeChanging(System.Nullable<int> value);
    partial void OnBarCodeChanged();
    partial void OnTableIdChanging(System.Nullable<int> value);
    partial void OnTableIdChanged();
    partial void OnQueueIdChanging(System.Nullable<int> value);
    partial void OnQueueIdChanged();
    partial void OnEmployeeIdChanging(System.Nullable<int> value);
    partial void OnEmployeeIdChanged();
    partial void OnModeIdChanging(System.Nullable<int> value);
    partial void OnModeIdChanged();
    partial void OnEntryIdChanging(System.Nullable<int> value);
    partial void OnEntryIdChanged();
    partial void OnSaleChanging(System.Nullable<bool> value);
    partial void OnSaleChanged();
    partial void OnEmployeeOwnerChanging(System.Nullable<int> value);
    partial void OnEmployeeOwnerChanged();
    partial void OnDishClosedChanging(bool value);
    partial void OnDishClosedChanged();
    #endregion
		
		public OrderTime()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CheckId", DbType="Int")]
		public System.Nullable<int> CheckId
		{
			get
			{
				return this._CheckId;
			}
			set
			{
				if ((this._CheckId != value))
				{
					this.OnCheckIdChanging(value);
					this.SendPropertyChanging();
					this._CheckId = value;
					this.SendPropertyChanged("CheckId");
					this.OnCheckIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_BusinessDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> BusinessDate
		{
			get
			{
				return this._BusinessDate;
			}
			set
			{
				if ((this._BusinessDate != value))
				{
					this.OnBusinessDateChanging(value);
					this.SendPropertyChanging();
					this._BusinessDate = value;
					this.SendPropertyChanged("BusinessDate");
					this.OnBusinessDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_SystemDate", DbType="DateTime")]
		public System.Nullable<System.DateTime> SystemDate
		{
			get
			{
				return this._SystemDate;
			}
			set
			{
				if ((this._SystemDate != value))
				{
					this.OnSystemDateChanging(value);
					this.SendPropertyChanging();
					this._SystemDate = value;
					this.SendPropertyChanged("SystemDate");
					this.OnSystemDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_BarCode", DbType="Int")]
		public System.Nullable<int> BarCode
		{
			get
			{
				return this._BarCode;
			}
			set
			{
				if ((this._BarCode != value))
				{
					this.OnBarCodeChanging(value);
					this.SendPropertyChanging();
					this._BarCode = value;
					this.SendPropertyChanged("BarCode");
					this.OnBarCodeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TableId", DbType="Int")]
		public System.Nullable<int> TableId
		{
			get
			{
				return this._TableId;
			}
			set
			{
				if ((this._TableId != value))
				{
					this.OnTableIdChanging(value);
					this.SendPropertyChanging();
					this._TableId = value;
					this.SendPropertyChanged("TableId");
					this.OnTableIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_QueueId", DbType="Int")]
		public System.Nullable<int> QueueId
		{
			get
			{
				return this._QueueId;
			}
			set
			{
				if ((this._QueueId != value))
				{
					this.OnQueueIdChanging(value);
					this.SendPropertyChanging();
					this._QueueId = value;
					this.SendPropertyChanged("QueueId");
					this.OnQueueIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EmployeeId", DbType="Int")]
		public System.Nullable<int> EmployeeId
		{
			get
			{
				return this._EmployeeId;
			}
			set
			{
				if ((this._EmployeeId != value))
				{
					this.OnEmployeeIdChanging(value);
					this.SendPropertyChanging();
					this._EmployeeId = value;
					this.SendPropertyChanged("EmployeeId");
					this.OnEmployeeIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ModeId", DbType="Int")]
		public System.Nullable<int> ModeId
		{
			get
			{
				return this._ModeId;
			}
			set
			{
				if ((this._ModeId != value))
				{
					this.OnModeIdChanging(value);
					this.SendPropertyChanging();
					this._ModeId = value;
					this.SendPropertyChanged("ModeId");
					this.OnModeIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EntryId", DbType="Int")]
		public System.Nullable<int> EntryId
		{
			get
			{
				return this._EntryId;
			}
			set
			{
				if ((this._EntryId != value))
				{
					this.OnEntryIdChanging(value);
					this.SendPropertyChanging();
					this._EntryId = value;
					this.SendPropertyChanged("EntryId");
					this.OnEntryIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Sale", DbType="Bit")]
		public System.Nullable<bool> Sale
		{
			get
			{
				return this._Sale;
			}
			set
			{
				if ((this._Sale != value))
				{
					this.OnSaleChanging(value);
					this.SendPropertyChanging();
					this._Sale = value;
					this.SendPropertyChanged("Sale");
					this.OnSaleChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EmployeeOwner", DbType="Int")]
		public System.Nullable<int> EmployeeOwner
		{
			get
			{
				return this._EmployeeOwner;
			}
			set
			{
				if ((this._EmployeeOwner != value))
				{
					this.OnEmployeeOwnerChanging(value);
					this.SendPropertyChanging();
					this._EmployeeOwner = value;
					this.SendPropertyChanged("EmployeeOwner");
					this.OnEmployeeOwnerChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DishClosed", DbType="Bit NOT NULL")]
		public bool DishClosed
		{
			get
			{
				return this._DishClosed;
			}
			set
			{
				if ((this._DishClosed != value))
				{
					this.OnDishClosedChanging(value);
					this.SendPropertyChanging();
					this._DishClosed = value;
					this.SendPropertyChanged("DishClosed");
					this.OnDishClosedChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
