using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	internal class Basic_SMap : EntityTypeConfiguration<Basic_S>
	{
		public Basic_SMap()
		{
			// Primary Key
			this.HasKey(t => t.Id);

			this.Property(t => t.Id)
				.IsRequired()
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

			this.Property(t => t.CompanyFullN)
				.HasMaxLength(255)
				.IsUnicode(true);

			this.Property(t => t.CompanyShortN)
				.HasMaxLength(100)
				.IsUnicode(true);

			this.Property(t => t.Address1)
				.HasMaxLength(255)
				.IsUnicode(true);

			this.Property(t => t.Address2)
				.HasMaxLength(255)
				.IsUnicode(true);

			this.Property(t => t.PhoneNumber1)
				.HasMaxLength(15)
				.IsUnicode(true);

			this.Property(t => t.PhoneNumber2)
				.HasMaxLength(15)
				.IsUnicode(true);

			this.Property(t => t.Fax)
				.HasMaxLength(15)
				.IsUnicode(false);

			this.Property(t => t.TaxCode)
				.HasMaxLength(15)
				.IsUnicode(false);

			this.Property(t => t.ContactPerson)
				.HasMaxLength(50)
				.IsUnicode(true);

			this.Property(t => t.Email)
				.HasMaxLength(50)
				.IsUnicode(false);

			this.Property(t => t.Language)
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.Logo)
				.HasMaxLength(255)
				.IsUnicode(true);

			this.Property(t => t.TaxRate)
				.HasPrecision(3, 1);

			// TaxRate and SettlementDay
			this.Property(t => t.TaxRoundingI)
			.HasMaxLength(1)
			.IsUnicode(false);

			this.Property(t => t.TaxMethodI)
			.HasMaxLength(1)
			.IsUnicode(false);

			this.Property(t => t.RevenueRoundingI)
			.HasMaxLength(1)
			.IsUnicode(false);

			this.Property(t => t.StatusColor1)
			.HasMaxLength(7)
			.IsUnicode(false);

			this.Property(t => t.StatusColor2)
			.HasMaxLength(7)
			.IsUnicode(false);

			this.Property(t => t.StatusColor3)
			.HasMaxLength(7)
			.IsUnicode(false);

			this.Property(t => t.StatusColor4)
			.HasMaxLength(7)
			.IsUnicode(false);

			this.Property(t => t.DetentionAmount)
				.HasPrecision(10, 0);

			this.Property(t => t.Expense1)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Expense2)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Expense3)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Expense4)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Expense5)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Expense6)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Expense7)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Expense8)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Expense9)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Expense10)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Allowance1)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Allowance1)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Allowance2)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Allowance3)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Allowance4)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Allowance5)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Allowance6)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Allowance7)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Allowance8)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Allowance9)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Allowance10)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Surcharge1)
			.HasMaxLength(5)
			.IsUnicode(false);


			this.Property(t => t.Surcharge2)
			.HasMaxLength(5)
			.IsUnicode(false);


			this.Property(t => t.Surcharge3)
			.HasMaxLength(5)
			.IsUnicode(false);


			this.Property(t => t.Surcharge4)
			.HasMaxLength(5)
			.IsUnicode(false);


			this.Property(t => t.Surcharge5)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Surcharge6)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Surcharge7)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Surcharge8)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Surcharge9)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.Surcharge10)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerCost1)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerCost2)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerCost3)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerCost4)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerCost5)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerCost6)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerCost7)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerCost8)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerCost9)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerCost10)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerSurcharge1)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerSurcharge2)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerSurcharge3)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerSurcharge4)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerSurcharge5)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerSurcharge6)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerSurcharge7)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerSurcharge8)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerSurcharge9)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.PartnerSurcharge10)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.FuelExpense1)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.FuelExpense2)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.FuelExpense3)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.FuelExpense4)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.FuelExpense5)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.FuelExpense6)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.FuelExpense7)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.FuelExpense8)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.FuelExpense9)
			.HasMaxLength(5)
			.IsUnicode(false);

			this.Property(t => t.FuelExpense10)
			.HasMaxLength(5)
			.IsUnicode(false);

            this.Property(t => t.ContainerStatus)
            .IsUnicode(false)
            .IsFixedLength()
            .HasMaxLength(1);

			this.Property(t => t.DisplaySalary)
			.IsUnicode(false)
			.IsFixedLength()
			.HasMaxLength(1);
			// Table & Column Mappings
			this.ToTable("Basic_S");
			this.Property(t => t.Id).HasColumnName("Id");
			this.Property(t => t.CompanyFullN).HasColumnName("CompanyFullN");
			this.Property(t => t.CompanyShortN).HasColumnName("CompanyShortN");
			this.Property(t => t.Address1).HasColumnName("Address1");
			this.Property(t => t.Address2).HasColumnName("Address2");
			this.Property(t => t.PhoneNumber1).HasColumnName("PhoneNumber1");
			this.Property(t => t.PhoneNumber2).HasColumnName("PhoneNumber2");
			this.Property(t => t.Fax).HasColumnName("Fax");
			this.Property(t => t.TaxCode).HasColumnName("TaxCode");
			this.Property(t => t.ContactPerson).HasColumnName("ContactPerson");
			this.Property(t => t.Email).HasColumnName("Email");
			this.Property(t => t.Language).HasColumnName("Language");
			this.Property(t => t.Logo).HasColumnName("Logo");
			this.Property(t => t.TaxRate).HasColumnName("TaxRate");
			this.Property(t => t.SettlementDay).HasColumnName("SettlementDay");
			this.Property(t => t.TaxRoundingI).HasColumnName("TaxRoundingI");
			this.Property(t => t.TaxMethodI).HasColumnName("TaxMethodI");
			this.Property(t => t.RevenueRoundingI).HasColumnName("RevenueRoundingI");
			this.Property(t => t.WarningCutOff).HasColumnName("WarningCutOff");
			this.Property(t => t.StatusColor1).HasColumnName("StatusColor1");
			this.Property(t => t.StatusColor2).HasColumnName("StatusColor2");
			this.Property(t => t.StatusColor3).HasColumnName("StatusColor3");
			this.Property(t => t.StatusColor4).HasColumnName("StatusColor4");
			this.Property(t => t.DispatchNo).HasColumnName("DispatchNo");
			this.Property(t => t.BeginDetentionDay).HasColumnName("BeginDetentionDay");
			this.Property(t => t.AccessTokenTimeout).HasColumnName("AccessTokenTimeout");
			this.Property(t => t.DetentionAmount).HasColumnName("DetentionAmount");
			this.Property(t => t.IsCalendarBeginningPage).HasColumnName("IsCalendarBeginningPage");
			this.Property(t => t.BackupNotificationDay).HasColumnName("BackupNotificationDay");
			this.Property(t => t.BackupPlanDay).HasColumnName("BackupPlanDay");
			this.Property(t => t.Expense1).HasColumnName("Expense1");
			this.Property(t => t.Expense2).HasColumnName("Expense2");
			this.Property(t => t.Expense3).HasColumnName("Expense3");
			this.Property(t => t.Expense4).HasColumnName("Expense4");
			this.Property(t => t.Expense5).HasColumnName("Expense5");
			this.Property(t => t.Expense6).HasColumnName("Expense6");
			this.Property(t => t.Expense7).HasColumnName("Expense7");
			this.Property(t => t.Expense8).HasColumnName("Expense8");
			this.Property(t => t.Expense9).HasColumnName("Expense9");
			this.Property(t => t.Expense10).HasColumnName("Expense10");
			this.Property(t => t.Allowance1).HasColumnName("Allowance1");
			this.Property(t => t.Allowance2).HasColumnName("Allowance2");
			this.Property(t => t.Allowance3).HasColumnName("Allowance3");
			this.Property(t => t.Allowance4).HasColumnName("Allowance4");
			this.Property(t => t.Allowance5).HasColumnName("Allowance5");
			this.Property(t => t.Allowance6).HasColumnName("Allowance6");
			this.Property(t => t.Allowance7).HasColumnName("Allowance7");
			this.Property(t => t.Allowance8).HasColumnName("Allowance8");
			this.Property(t => t.Allowance9).HasColumnName("Allowance9");
			this.Property(t => t.Allowance10).HasColumnName("Allowance10");
			this.Property(t => t.Surcharge1).HasColumnName("Surcharge1");
			this.Property(t => t.Surcharge2).HasColumnName("Surcharge2");
			this.Property(t => t.Surcharge3).HasColumnName("Surcharge3");
			this.Property(t => t.Surcharge4).HasColumnName("Surcharge4");
			this.Property(t => t.Surcharge5).HasColumnName("Surcharge5");
			this.Property(t => t.Surcharge6).HasColumnName("Surcharge6");
			this.Property(t => t.Surcharge7).HasColumnName("Surcharge7");
			this.Property(t => t.Surcharge8).HasColumnName("Surcharge8");
			this.Property(t => t.Surcharge9).HasColumnName("Surcharge9");
			this.Property(t => t.Surcharge10).HasColumnName("Surcharge10");
			this.Property(t => t.PartnerCost1).HasColumnName("PartnerCost1");
			this.Property(t => t.PartnerCost2).HasColumnName("PartnerCost2");
			this.Property(t => t.PartnerCost3).HasColumnName("PartnerCost3");
			this.Property(t => t.PartnerCost4).HasColumnName("PartnerCost4");
			this.Property(t => t.PartnerCost5).HasColumnName("PartnerCost5");
			this.Property(t => t.PartnerCost6).HasColumnName("PartnerCost6");
			this.Property(t => t.PartnerCost7).HasColumnName("PartnerCost7");
			this.Property(t => t.PartnerCost8).HasColumnName("PartnerCost8");
			this.Property(t => t.PartnerCost9).HasColumnName("PartnerCost9");
			this.Property(t => t.PartnerCost10).HasColumnName("PartnerCost10");
			this.Property(t => t.PartnerSurcharge1).HasColumnName("PartnerSurcharge1");
			this.Property(t => t.PartnerSurcharge2).HasColumnName("PartnerSurcharge2");
			this.Property(t => t.PartnerSurcharge3).HasColumnName("PartnerSurcharge3");
			this.Property(t => t.PartnerSurcharge4).HasColumnName("PartnerSurcharge4");
			this.Property(t => t.PartnerSurcharge5).HasColumnName("PartnerSurcharge5");
			this.Property(t => t.PartnerSurcharge6).HasColumnName("PartnerSurcharge6");
			this.Property(t => t.PartnerSurcharge7).HasColumnName("PartnerSurcharge7");
			this.Property(t => t.PartnerSurcharge8).HasColumnName("PartnerSurcharge8");
			this.Property(t => t.PartnerSurcharge9).HasColumnName("PartnerSurcharge9");
			this.Property(t => t.PartnerSurcharge10).HasColumnName("PartnerSurcharge10");
			this.Property(t => t.FuelExpense1).HasColumnName("FuelExpense1");
			this.Property(t => t.FuelExpense2).HasColumnName("FuelExpense2");
			this.Property(t => t.FuelExpense3).HasColumnName("FuelExpense3");
			this.Property(t => t.FuelExpense4).HasColumnName("FuelExpense4");
			this.Property(t => t.FuelExpense5).HasColumnName("FuelExpense5");
			this.Property(t => t.FuelExpense6).HasColumnName("FuelExpense6");
			this.Property(t => t.FuelExpense7).HasColumnName("FuelExpense7");
			this.Property(t => t.FuelExpense8).HasColumnName("FuelExpense8");
			this.Property(t => t.FuelExpense9).HasColumnName("FuelExpense9");
			this.Property(t => t.FuelExpense10).HasColumnName("FuelExpense10");
            this.Property(t => t.ContainerStatus).HasColumnName("ContainerStatus");
			this.Property(t => t.DisplaySalary).HasColumnName("DisplaySalary");
		}
	}
}