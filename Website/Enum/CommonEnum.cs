using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.Enum
{
	public class Constants
	{
		public const string ACTIVE = "1";
		public const string DEACTIVE = "0";

		public const string EXP = "0";
		public const string IMP = "1";
		public const string ETC = "2";

		public const string EXPNAME = "Exp";
		public const string IMPNAME = "Imp";
		public const string ETCNAME = "Etc";

		public const string CONTAINERSIZE1 = "0";
		public const string CONTAINERSIZE2 = "1";
		public const string CONTAINERSIZE3 = "2";

		public const string CONTAINERSIZE1N = "20'";
		public const string CONTAINERSIZE2N = "40'";
		public const string CONTAINERSIZE3N = "45'";

		//Dispatch Status
		public const string NOTDISPATCH = "0";
		public const string DISPATCH = "1";
		public const string TRANSPORTED = "2";
		public const string CONFIRMED = "3";
		//Container Status
		public const string NORMAL = "0";
		public const string LOAD = "1";
		public const string DISCHARGE = "2";

		public const string CATEGORYI1 = "1";
		public const string CATEGORYI2 = "2";
		public const string CATEGORYI3 = "3";
		public const string CATEGORYI4 = "4";
		public const string CATEGORYI0 = "0";

		public const string ISINCLUDED1 = "1";
		public const string ISINCLUDED2 = "0";

		public const string ISPAYABLE1 = "1";
		public const string ISPAYABLE2 = "0";
		// PaymentMethod
		public const string DEBIT = "1";
		public const string CASH = "0";
		public const string TRANSFER = "2";
		// ObjectI
		public const string TRUCK = "0";
		public const string TRAILER = "1";
		//FormOfSettlement
		public const string SETTLEMENT = "1";
	}

	public enum Screen
	{
		Common = 1,
		Customer = 2,
		Order = 3,
		Dispatch = 13,
		TransportConfirm = 14
	}

	public enum Status
	{
		Deactive = 0,
		Active = 1,
	}

	public enum ExpenseCategory
	{
		Other = 0,
		Expense = 1,
		Surcharge = 2,
		Allowance = 3,
		Fix = 4
	}

	public enum DriverAllowanceMethod
	{
		Location = 0,
		Percent = 1,
		Distance = 2,
		HandInput = 3
	}

	public enum DispatchStatus
	{
		NotDispatch = 0,
		Dispatch = 1,
		Transported = 2,
		Confirmed = 3
	}

	public enum ContainerStatus
	{
		Normal = 0,
		Load = 1,
		Discharge = 2,
	}

	public enum DeleteLevel
	{
		NotDeleted = 0,
		NotDeletedAndWarning = 1,
		Deleted = 2
	}

	public enum InvoiceStatus
	{
		NotIssue = 0,
		Issued = 1,
	}

	public enum PlanStatus
	{
		LatePlan = 0,
		OnPlan = 1,
		EarlyPlan = 2
	}

	public enum PlanUnit
	{
		Day = 0,
		Km = 1,
	}

	public enum PlanItemStatus
	{
		Undone = 0,
		Done = 1,
	}

	public enum PlanType
	{
		License = 0,
		Maintainence = 1,
		Inspection = 2,
	}

	public enum FormStatus
	{
		Add = 1,
		Edit = 2,
		Delete = 3,
		Reset = 4,
	}

	public enum ApplicationLicense
	{
		WrongLicense = 1,
		OverTruckLimitation = 2,
	}

	public enum ExpenseRoot
	{
		Manual = 1,
		Route = 2,
		History = 3,
	}

	public enum DispatchType {
		Truck = 0,
		PartnerTruck = 1
	}
}