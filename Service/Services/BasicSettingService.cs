using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Service.Services;
using Website.Enum;
using Website.Utilities;
using Website.ViewModels.Basic;
using Website.ViewModels.Expense;

namespace Service.Services
{
	public interface IBasicSettingService
	{
		List<BasicViewModel> GetBasicSetting(string licensePath, string backupInfoPath);
		List<ExpenseViewModel> GetExpenseCodesFromBasicSetting(string categoryI, string dispatchI);
		string GetDispatchTransportedColor();
		void CreateBasicSetting(BasicViewModel basicsetting);
        string GetContainerStatus(DateTime orderD, string orderNo, int detailNo);	}
	}
	public class BasicSettingService :IBasicSettingService
	{
        private readonly IDispatchRepository _dispatchRepository;
		private readonly IBasicRepository _basicRepository;
		private readonly ILicenseValidation _licenseValidation;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IExpenseRepository _expenseRepository;
		private readonly IExpenseCategoryRepository _expenseCategoryRepository;
		private readonly IBackupRestoreDbService _backupRestoreDbService;

		public BasicSettingService(IDispatchRepository dispatchRepository, IBasicRepository basicRepository, ILicenseValidation licenseValidation,
									IUnitOfWork unitOfWork, IExpenseRepository expenseRepository, 
									IExpenseCategoryRepository expeseCategoryRepository, IBackupRestoreDbService backupRestoreDbService)
		{
            this._dispatchRepository = dispatchRepository;
			this._basicRepository = basicRepository;
			this._licenseValidation = licenseValidation;
			this._unitOfWork = unitOfWork;
			this._expenseRepository = expenseRepository;
			this._expenseCategoryRepository = expeseCategoryRepository;
			this._backupRestoreDbService = backupRestoreDbService;
		}
		public void SaveBasicSetting()
		{
			_unitOfWork.Commit();
		}
		public void CreateBasicSetting(BasicViewModel basicsettingViewModel)
		{
			try
			{
				//add value on expense
				var iloop = 0;
				var expenseList = basicsettingViewModel.Expenses;
				var surchargeList = basicsettingViewModel.Surcharges;
				var allowanceList = basicsettingViewModel.Allowances;
				var partnercostList = basicsettingViewModel.PartnerCosts;
				var partnersurchargeList = basicsettingViewModel.PartnerSurcharges;
				var fuelexpenseList = basicsettingViewModel.FuelExpenses;
				ResetExpense(basicsettingViewModel);
				if (expenseList != null && expenseList.Count > 0)
				{
					for (iloop = 0; iloop < expenseList.Count; iloop++)
					{
						switch (iloop)
						{
							case 0:
								basicsettingViewModel.Expense1 = expenseList[iloop].ExpenseC;
								break;
							case 1:
								basicsettingViewModel.Expense2 = expenseList[iloop].ExpenseC;
								break;
							case 2:
								basicsettingViewModel.Expense3 = expenseList[iloop].ExpenseC;
								break;
							case 3:
								basicsettingViewModel.Expense4 = expenseList[iloop].ExpenseC;
								break;
							case 4:
								basicsettingViewModel.Expense5 = expenseList[iloop].ExpenseC;
								break;
							case 5:
								basicsettingViewModel.Expense6 = expenseList[iloop].ExpenseC;
								break;
							case 6:
								basicsettingViewModel.Expense7 = expenseList[iloop].ExpenseC;
								break;
							case 7:
								basicsettingViewModel.Expense8 = expenseList[iloop].ExpenseC;
								break;
							case 8:
								basicsettingViewModel.Expense9 = expenseList[iloop].ExpenseC;
								break;
							case 9:
								basicsettingViewModel.Expense10 = expenseList[iloop].ExpenseC;
								break;
						}
						
					}
				}
				//add value on surcharge
				if (surchargeList != null && surchargeList.Count > 0)
				{
					for (iloop = 0; iloop < surchargeList.Count; iloop++)
					{
						switch (iloop)
						{
							case 0:
								basicsettingViewModel.Surcharge1 = surchargeList[iloop].ExpenseC;
								break;
							case 1:
								basicsettingViewModel.Surcharge2 = surchargeList[iloop].ExpenseC;
								break;
							case 2:
								basicsettingViewModel.Surcharge3 = surchargeList[iloop].ExpenseC;
								break;
							case 3:
								basicsettingViewModel.Surcharge4 = surchargeList[iloop].ExpenseC;
								break;
							case 4:
								basicsettingViewModel.Surcharge5 = surchargeList[iloop].ExpenseC;
								break;
							case 5:
								basicsettingViewModel.Surcharge6 = surchargeList[iloop].ExpenseC;
								break;
							case 6:
								basicsettingViewModel.Surcharge7 = surchargeList[iloop].ExpenseC;
								break;
							case 7:
								basicsettingViewModel.Surcharge8 = surchargeList[iloop].ExpenseC;
								break;
							case 8:
								basicsettingViewModel.Surcharge9 = surchargeList[iloop].ExpenseC;
								break;
							case 9:
								basicsettingViewModel.Surcharge10 = surchargeList[iloop].ExpenseC;
								break;
						}
					}
				}
				//add value on allowance
				if (allowanceList != null && allowanceList.Count > 0)
				{
					for (iloop = 0; iloop < allowanceList.Count; iloop++)
					{
						switch (iloop)
						{
							case 0:
								basicsettingViewModel.Allowance1 = allowanceList[iloop].ExpenseC;
								break;
							case 1:
								basicsettingViewModel.Allowance2 = allowanceList[iloop].ExpenseC;
								break;
							case 2:
								basicsettingViewModel.Allowance3 = allowanceList[iloop].ExpenseC;
								break;
							case 3:
								basicsettingViewModel.Allowance4 = allowanceList[iloop].ExpenseC;
								break;
							case 4:
								basicsettingViewModel.Allowance5 = allowanceList[iloop].ExpenseC;
								break;
							case 5:
								basicsettingViewModel.Allowance6 = allowanceList[iloop].ExpenseC;
								break;
							case 6:
								basicsettingViewModel.Allowance7 = allowanceList[iloop].ExpenseC;
								break;
							case 7:
								basicsettingViewModel.Allowance8 = allowanceList[iloop].ExpenseC;
								break;
							case 8:
								basicsettingViewModel.Allowance9 = allowanceList[iloop].ExpenseC;
								break;
							case 9:
								basicsettingViewModel.Allowance10 = allowanceList[iloop].ExpenseC;
								break;
						}
					}
				}
				//add value on partnercost
				if (partnercostList != null && partnercostList.Count > 0)
				{
					for (iloop = 0; iloop < partnercostList.Count; iloop++)
					{
						switch (iloop)
						{
							case 0:
								basicsettingViewModel.PartnerCost1 = partnercostList[iloop].ExpenseC;
								break;
							case 1:
								basicsettingViewModel.PartnerCost2 = partnercostList[iloop].ExpenseC;
								break;
							case 2:
								basicsettingViewModel.PartnerCost3 = partnercostList[iloop].ExpenseC;
								break;
							case 3:
								basicsettingViewModel.PartnerCost4 = partnercostList[iloop].ExpenseC;
								break;
							case 4:
								basicsettingViewModel.PartnerCost5 = partnercostList[iloop].ExpenseC;
								break;
							case 5:
								basicsettingViewModel.PartnerCost6 = partnercostList[iloop].ExpenseC;
								break;
							case 6:
								basicsettingViewModel.PartnerCost7 = partnercostList[iloop].ExpenseC;
								break;
							case 7:
								basicsettingViewModel.PartnerCost8 = partnercostList[iloop].ExpenseC;
								break;
							case 8:
								basicsettingViewModel.PartnerCost9 = partnercostList[iloop].ExpenseC;
								break;
							case 9:
								basicsettingViewModel.PartnerCost10 = partnercostList[iloop].ExpenseC;
								break;
						}
					}
				}
				//add value on partnersurcharge
				if (partnersurchargeList != null && partnersurchargeList.Count > 0)
				{
					for (iloop = 0; iloop < partnersurchargeList.Count; iloop++)
					{
						switch (iloop)
						{
							case 0:
								basicsettingViewModel.PartnerSurcharge1 = partnersurchargeList[iloop].ExpenseC;
								break;
							case 1:
								basicsettingViewModel.PartnerSurcharge2 = partnersurchargeList[iloop].ExpenseC;
								break;
							case 2:
								basicsettingViewModel.PartnerSurcharge3 = partnersurchargeList[iloop].ExpenseC;
								break;
							case 3:
								basicsettingViewModel.PartnerSurcharge4 = partnersurchargeList[iloop].ExpenseC;
								break;
							case 4:
								basicsettingViewModel.PartnerSurcharge5 = partnersurchargeList[iloop].ExpenseC;
								break;
							case 5:
								basicsettingViewModel.PartnerSurcharge6 = partnersurchargeList[iloop].ExpenseC;
								break;
							case 6:
								basicsettingViewModel.PartnerSurcharge7 = partnersurchargeList[iloop].ExpenseC;
								break;
							case 7:
								basicsettingViewModel.PartnerSurcharge8 = partnersurchargeList[iloop].ExpenseC;
								break;
							case 8:
								basicsettingViewModel.PartnerSurcharge9 = partnersurchargeList[iloop].ExpenseC;
								break;
							case 9:
								basicsettingViewModel.PartnerSurcharge10 = partnersurchargeList[iloop].ExpenseC;
								break;
						}
					}
				}
				//add value on fuel expense
				if (fuelexpenseList != null && fuelexpenseList.Count > 0)
				{
					for (iloop = 0; iloop < fuelexpenseList.Count; iloop++)
					{
						switch (iloop)
						{
							case 0:
								basicsettingViewModel.FuelExpense1 = fuelexpenseList[iloop].CategoryC;
								break;
							case 1:
								basicsettingViewModel.FuelExpense2 = fuelexpenseList[iloop].CategoryC;
								break;
							case 2:
								basicsettingViewModel.FuelExpense3 = fuelexpenseList[iloop].CategoryC;
								break;
							case 3:
								basicsettingViewModel.FuelExpense4 = fuelexpenseList[iloop].CategoryC;
								break;
							case 4:
								basicsettingViewModel.FuelExpense5 = fuelexpenseList[iloop].CategoryC;
								break;
							case 5:
								basicsettingViewModel.FuelExpense6 = fuelexpenseList[iloop].CategoryC;
								break;
							case 6:
								basicsettingViewModel.FuelExpense7 = fuelexpenseList[iloop].CategoryC;
								break;
							case 7:
								basicsettingViewModel.FuelExpense8 = fuelexpenseList[iloop].CategoryC;
								break;
							case 8:
								basicsettingViewModel.FuelExpense9 = fuelexpenseList[iloop].CategoryC;
								break;
							case 9:
								basicsettingViewModel.FuelExpense10 = fuelexpenseList[iloop].CategoryC;
								break;
						}
					}
				}
				var basicsetting = Mapper.Map<BasicViewModel, Basic_S>(basicsettingViewModel);
				// delete all data in Basic_S table
				var source = _basicRepository.GetAllQueryable();
				var result = source.ToList();
				if (result.Count > 0)
				{
					basicsetting.Id = result[0].Id;
					_basicRepository.Detach(result[0]);
					_basicRepository.Update(basicsetting);
				}
				else
				{
					//insert data into database
					_basicRepository.Add(basicsetting);
				}
				
				SaveBasicSetting();
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}
		public List<BasicViewModel> GetBasicSetting(string licensePath, string backupInfoPath)
		{
			var source = _basicRepository.GetAllQueryable();
			var destination = Mapper.Map<IEnumerable<Basic_S>, IEnumerable<BasicViewModel>>(source);
			var result = destination.ToList();

			if (result.Count <= 0) return result;
			var basic = result[0];
			basic.Expenses = new List<ExpenseViewModel>();
			basic.Surcharges = new List<ExpenseViewModel>();
			basic.Allowances = new List<ExpenseViewModel>();
			basic.PartnerCosts = new List<ExpenseViewModel>();
			basic.PartnerSurcharges = new List<ExpenseViewModel>();
			basic.FuelExpenses = new List<ExpenseCategoryViewModel>();
			#region get expense
			//expense
			var expense = new ExpenseViewModel();
			if (basic.Expense1 != null)
			{
				// set expenseC
				var expenseC = basic.Expense1;
				expense.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				expense.ExpenseN = "";
				if (expenseData != null)
				{
					expense.ExpenseN = expenseData.ExpenseN;
				}
					
				basic.Expenses.Add(expense);
			}
			if (basic.Expense2 != null)
			{
				expense = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Expense2;
				expense.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				expense.ExpenseN = "";
				if (expenseData != null)
				{
					expense.ExpenseN = expenseData.ExpenseN;
				}
				basic.Expenses.Add(expense);
			}
			if (basic.Expense3 != null)
			{
				expense = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Expense3;
				expense.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				expense.ExpenseN = "";
				if (expenseData != null)
				{
					expense.ExpenseN = expenseData.ExpenseN;
				}
				basic.Expenses.Add(expense);
			}
			if (basic.Expense4 != null)
			{
				expense = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Expense4;
				expense.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				expense.ExpenseN = "";
				if (expenseData != null)
				{
					expense.ExpenseN = expenseData.ExpenseN;
				}
				basic.Expenses.Add(expense);
			}
			if (basic.Expense5 != null)
			{
				expense = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Expense5;
				expense.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				expense.ExpenseN = "";
				if (expenseData != null)
				{
					expense.ExpenseN = expenseData.ExpenseN;
				}
				basic.Expenses.Add(expense);
			}
			if (basic.Expense6 != null)
			{
				expense = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Expense6;
				expense.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				expense.ExpenseN = "";
				if (expenseData != null)
				{
					expense.ExpenseN = expenseData.ExpenseN;
				}
				basic.Expenses.Add(expense);
			}
			if (basic.Expense7 != null)
			{
				expense = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Expense7;
				expense.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				expense.ExpenseN = "";
				if (expenseData != null)
				{
					expense.ExpenseN = expenseData.ExpenseN;
				}
				basic.Expenses.Add(expense);
			}
			if (basic.Expense8 != null)
			{
				expense = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Expense8;
				expense.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				expense.ExpenseN = "";
				if (expenseData != null)
				{
					expense.ExpenseN = expenseData.ExpenseN;
				}
				basic.Expenses.Add(expense);
			}
			if (basic.Expense9 != null)
			{
				expense = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Expense9;
				expense.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				expense.ExpenseN = "";
				if (expenseData != null)
				{
					expense.ExpenseN = expenseData.ExpenseN;
				}
				basic.Expenses.Add(expense);
			}
			if (basic.Expense10 != null)
			{
				expense = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Expense10;
				expense.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				expense.ExpenseN = "";
				if (expenseData != null)
				{
					expense.ExpenseN = expenseData.ExpenseN;
				}
				basic.Expenses.Add(expense);
			}
			//surcharge
			var surcharge = new ExpenseViewModel();
			if(basic.Surcharge1 != null)
			{
				// set expenseC
				var expenseC = basic.Surcharge1;
				surcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				surcharge.ExpenseN = "";
				if (expenseData != null)
				{
					surcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.Surcharges.Add(surcharge);
			}
			if (basic.Surcharge2 != null)
			{
				surcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Surcharge2;
				surcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				surcharge.ExpenseN = "";
				if (expenseData != null)
				{
					surcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.Surcharges.Add(surcharge);
			}
			if (basic.Surcharge3 != null)
			{
				surcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Surcharge3;
				surcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				surcharge.ExpenseN = "";
				if (expenseData != null)
				{
					surcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.Surcharges.Add(surcharge);
			}
			if (basic.Surcharge4 != null)
			{
				surcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Surcharge4;
				surcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				surcharge.ExpenseN = "";
				if (expenseData != null)
				{
					surcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.Surcharges.Add(surcharge);
			}
			if (basic.Surcharge5 != null)
			{
				surcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Surcharge5;
				surcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				surcharge.ExpenseN = "";
				if (expenseData != null)
				{
					surcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.Surcharges.Add(surcharge);
			}
			if (basic.Surcharge6 != null)
			{
				surcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Surcharge6;
				surcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				surcharge.ExpenseN = "";
				if (expenseData != null)
				{
					surcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.Surcharges.Add(surcharge);
			}
			if (basic.Surcharge7 != null)
			{
				surcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Surcharge7;
				surcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				surcharge.ExpenseN = "";
				if (expenseData != null)
				{
					surcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.Surcharges.Add(surcharge);
			}
			if (basic.Surcharge8 != null)
			{
				surcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Surcharge8;
				surcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				surcharge.ExpenseN = "";
				if (expenseData != null)
				{
					surcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.Surcharges.Add(surcharge);
			}
			if (basic.Surcharge9 != null)
			{
				surcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Surcharge9;
				surcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				surcharge.ExpenseN = "";
				if (expenseData != null)
				{
					surcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.Surcharges.Add(surcharge);
			}
			if (basic.Surcharge10 != null)
			{
				surcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Surcharge10;
				surcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				surcharge.ExpenseN = "";
				if (expenseData != null)
				{
					surcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.Surcharges.Add(surcharge);
			}
			// allowance
			var allowance = new ExpenseViewModel();
			if (basic.Allowance1 != null)
			{
				// set expenseC
				var expenseC = basic.Allowance1;
				allowance.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				allowance.ExpenseN = "";
				if (expenseData != null)
				{
					allowance.ExpenseN = expenseData.ExpenseN;
				}
				basic.Allowances.Add(allowance);
			}
			if (basic.Allowance2 != null)
			{
				allowance = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Allowance2;
				allowance.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				allowance.ExpenseN = "";
				if (expenseData != null)
				{
					allowance.ExpenseN = expenseData.ExpenseN;
				}
				basic.Allowances.Add(allowance);
			}
			if (basic.Allowance3 != null)
			{
				allowance = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Allowance3;
				allowance.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				allowance.ExpenseN = "";
				if (expenseData != null)
				{
					allowance.ExpenseN = expenseData.ExpenseN;
				}
				basic.Allowances.Add(allowance);
			}
			if (basic.Allowance4 != null)
			{
				allowance = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Allowance4;
				allowance.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				allowance.ExpenseN = "";
				if (expenseData != null)
				{
					allowance.ExpenseN = expenseData.ExpenseN;
				}
				basic.Allowances.Add(allowance);
			}
			if (basic.Allowance5 != null)
			{
				allowance = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Allowance5;
				allowance.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				allowance.ExpenseN = "";
				if (expenseData != null)
				{
					allowance.ExpenseN = expenseData.ExpenseN;
				}
				basic.Allowances.Add(allowance);
			}
			if (basic.Allowance6 != null)
			{
				allowance = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Allowance6;
				allowance.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				allowance.ExpenseN = "";
				if (expenseData != null)
				{
					allowance.ExpenseN = expenseData.ExpenseN;
				}
				basic.Allowances.Add(allowance);
			}
			if (basic.Allowance7 != null)
			{
				allowance = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Allowance7;
				allowance.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				allowance.ExpenseN = "";
				if (expenseData != null)
				{
					allowance.ExpenseN = expenseData.ExpenseN;
				}
				basic.Allowances.Add(allowance);
			}
			if (basic.Allowance8 != null)
			{
				allowance = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Allowance8;
				allowance.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				allowance.ExpenseN = "";
				if (expenseData != null)
				{
					allowance.ExpenseN = expenseData.ExpenseN;
				}
				basic.Allowances.Add(allowance);
			}
			if (basic.Allowance9 != null)
			{
				allowance = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Allowance9;
				allowance.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				allowance.ExpenseN = "";
				if (expenseData != null)
				{
					allowance.ExpenseN = expenseData.ExpenseN;
				}
				basic.Allowances.Add(allowance);
			}
			if (basic.Allowance10 != null)
			{
				allowance = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.Allowance10;
				allowance.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				allowance.ExpenseN = "";
				if (expenseData != null)
				{
					allowance.ExpenseN = expenseData.ExpenseN;
				}
				basic.Allowances.Add(allowance);
			}
			//partnercost
			var partnercost = new ExpenseViewModel();
			if (basic.PartnerCost1 != null)
			{
				// set expenseC
				var expenseC = basic.PartnerCost1;
				partnercost.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnercost.ExpenseN = "";
				if (expenseData != null)
				{
					partnercost.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerCosts.Add(partnercost);
			}
			if (basic.PartnerCost2 != null)
			{
				partnercost = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerCost2;
				partnercost.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnercost.ExpenseN = "";
				if (expenseData != null)
				{
					partnercost.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerCosts.Add(partnercost);
			}
			if (basic.PartnerCost3 != null)
			{
				partnercost = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerCost3;
				partnercost.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnercost.ExpenseN = "";
				if (expenseData != null)
				{
					partnercost.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerCosts.Add(partnercost);
			}
			if (basic.PartnerCost4 != null)
			{
				partnercost = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerCost4;
				partnercost.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnercost.ExpenseN = "";
				if (expenseData != null)
				{
					partnercost.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerCosts.Add(partnercost);
			}
			if (basic.PartnerCost5 != null)
			{
				partnercost = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerCost5;
				partnercost.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnercost.ExpenseN = "";
				if (expenseData != null)
				{
					partnercost.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerCosts.Add(partnercost);
			}
			if (basic.PartnerCost6 != null)
			{
				partnercost = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerCost6;
				partnercost.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnercost.ExpenseN = "";
				if (expenseData != null)
				{
					partnercost.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerCosts.Add(partnercost);
			}
			if (basic.PartnerCost7 != null)
			{
				partnercost = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerCost7;
				partnercost.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnercost.ExpenseN = "";
				if (expenseData != null)
				{
					partnercost.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerCosts.Add(partnercost);
			}
			if (basic.PartnerCost8 != null)
			{
				partnercost = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerCost8;
				partnercost.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnercost.ExpenseN = "";
				if (expenseData != null)
				{
					partnercost.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerCosts.Add(partnercost);
			}
			if (basic.PartnerCost9 != null)
			{
				partnercost = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerCost9;
				partnercost.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnercost.ExpenseN = "";
				if (expenseData != null)
				{
					partnercost.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerCosts.Add(partnercost);
			}
			if (basic.PartnerCost10 != null)
			{
				partnercost = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerCost10;
				partnercost.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnercost.ExpenseN = "";
				if (expenseData != null)
				{
					partnercost.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerCosts.Add(partnercost);
			}
			//partnersurcharge
			var partnersurcharge = new ExpenseViewModel();
			if(basic.PartnerSurcharge1 != null)
			{
				// set expenseC
				var expenseC = basic.PartnerSurcharge1;
				partnersurcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnersurcharge.ExpenseN = "";
				if (expenseData != null)
				{
					partnersurcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerSurcharges.Add(partnersurcharge);
			}
			if (basic.PartnerSurcharge2 != null)
			{
				partnersurcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerSurcharge2;
				partnersurcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnersurcharge.ExpenseN = "";
				if (expenseData != null)
				{
					partnersurcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerSurcharges.Add(partnersurcharge);
			}
			if (basic.PartnerSurcharge3 != null)
			{
				partnersurcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerSurcharge3;
				partnersurcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnersurcharge.ExpenseN = "";
				if (expenseData != null)
				{
					partnersurcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerSurcharges.Add(partnersurcharge);
			}
			if (basic.PartnerSurcharge4 != null)
			{
				partnersurcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerSurcharge4;
				partnersurcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnersurcharge.ExpenseN = "";
				if (expenseData != null)
				{
					partnersurcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerSurcharges.Add(partnersurcharge);
			}
			if (basic.PartnerSurcharge5 != null)
			{
				partnersurcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerSurcharge5;
				partnersurcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnersurcharge.ExpenseN = "";
				if (expenseData != null)
				{
					partnersurcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerSurcharges.Add(partnersurcharge);
			}
			if (basic.PartnerSurcharge6 != null)
			{
				partnersurcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerSurcharge6;
				partnersurcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnersurcharge.ExpenseN = "";
				if (expenseData != null)
				{
					partnersurcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerSurcharges.Add(partnersurcharge);
			}
			if (basic.PartnerSurcharge7 != null)
			{
				partnersurcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerSurcharge7;
				partnersurcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnersurcharge.ExpenseN = "";
				if (expenseData != null)
				{
					partnersurcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerSurcharges.Add(partnersurcharge);
			}
			if (basic.PartnerSurcharge8 != null)
			{
				partnersurcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerSurcharge8;
				partnersurcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnersurcharge.ExpenseN = "";
				if (expenseData != null)
				{
					partnersurcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerSurcharges.Add(partnersurcharge);
			}
			if (basic.PartnerSurcharge9 != null)
			{
				partnersurcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerSurcharge9;
				partnersurcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnersurcharge.ExpenseN = "";
				if (expenseData != null)
				{
					partnersurcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerSurcharges.Add(partnersurcharge);
			}
			if (basic.PartnerSurcharge10 != null)
			{
				partnersurcharge = new ExpenseViewModel();
				// set expenseC
				var expenseC = basic.PartnerSurcharge10;
				partnersurcharge.ExpenseC = expenseC;
				// set expenseN
				var expenseData = _expenseRepository.Query(exp => exp.ExpenseC == expenseC).FirstOrDefault();
				partnersurcharge.ExpenseN = "";
				if (expenseData != null)
				{
					partnersurcharge.ExpenseN = expenseData.ExpenseN;
				}
				basic.PartnerSurcharges.Add(partnersurcharge);
			}
			var fuelExpense = new ExpenseCategoryViewModel();
			if (basic.FuelExpense1 != null)
			{
				// set expenseC
				var expenseC = basic.FuelExpense1;
				fuelExpense.CategoryC = expenseC;
				// set expenseN
				var fuelExpenseData = _expenseCategoryRepository.Query(exp => exp.CategoryC == expenseC).FirstOrDefault();
				fuelExpense.CategoryN = "";
				if (fuelExpenseData != null)
				{
					fuelExpense.CategoryN = fuelExpenseData.CategoryN;
				}
				basic.FuelExpenses.Add(fuelExpense);
			}
			if (basic.FuelExpense2 != null)
			{
				fuelExpense = new ExpenseCategoryViewModel();
				// set expenseC
				var expenseC = basic.FuelExpense2;
				fuelExpense.CategoryC = expenseC;
				// set expenseN
				var fuelExpenseData = _expenseCategoryRepository.Query(exp => exp.CategoryC == expenseC).FirstOrDefault();
				fuelExpense.CategoryN = "";
				if (fuelExpenseData != null)
				{
					fuelExpense.CategoryN = fuelExpenseData.CategoryN;
				}
				basic.FuelExpenses.Add(fuelExpense);
			}
			if (basic.FuelExpense3 != null)
			{
				fuelExpense = new ExpenseCategoryViewModel();
				// set expenseC
				var expenseC = basic.FuelExpense3;
				fuelExpense.CategoryC = expenseC;
				// set expenseN
				var fuelExpenseData = _expenseCategoryRepository.Query(exp => exp.CategoryC == expenseC).FirstOrDefault();
				fuelExpense.CategoryN = "";
				if (fuelExpenseData != null)
				{
					fuelExpense.CategoryN = fuelExpenseData.CategoryN;
				}
				basic.FuelExpenses.Add(fuelExpense);
			}
			if (basic.FuelExpense4 != null)
			{
				fuelExpense = new ExpenseCategoryViewModel();
				// set expenseC
				var expenseC = basic.FuelExpense4;
				fuelExpense.CategoryC = expenseC;
				// set expenseN
				var fuelExpenseData = _expenseCategoryRepository.Query(exp => exp.CategoryC == expenseC).FirstOrDefault();
				fuelExpense.CategoryN = "";
				if (fuelExpenseData != null)
				{
					fuelExpense.CategoryN = fuelExpenseData.CategoryN;
				}
				basic.FuelExpenses.Add(fuelExpense);
			}
			if (basic.FuelExpense5 != null)
			{
				fuelExpense = new ExpenseCategoryViewModel();
				// set expenseC
				var expenseC = basic.FuelExpense5;
				fuelExpense.CategoryC = expenseC;
				// set expenseN
				var fuelExpenseData = _expenseCategoryRepository.Query(exp => exp.CategoryC == expenseC).FirstOrDefault();
				fuelExpense.CategoryN = "";
				if (fuelExpenseData != null)
				{
					fuelExpense.CategoryN = fuelExpenseData.CategoryN;
				}
				basic.FuelExpenses.Add(fuelExpense);
			}
			if (basic.FuelExpense6 != null)
			{
				fuelExpense = new ExpenseCategoryViewModel();
				// set expenseC
				var expenseC = basic.FuelExpense6;
				fuelExpense.CategoryC = expenseC;
				// set expenseN
				var fuelExpenseData = _expenseCategoryRepository.Query(exp => exp.CategoryC == expenseC).FirstOrDefault();
				fuelExpense.CategoryN = "";
				if (fuelExpenseData != null)
				{
					fuelExpense.CategoryN = fuelExpenseData.CategoryN;
				}
				basic.FuelExpenses.Add(fuelExpense);
			}
			if (basic.FuelExpense7 != null)
			{
				fuelExpense = new ExpenseCategoryViewModel();
				// set expenseC
				var expenseC = basic.FuelExpense7;
				fuelExpense.CategoryC = expenseC;
				// set expenseN
				var fuelExpenseData = _expenseCategoryRepository.Query(exp => exp.CategoryC == expenseC).FirstOrDefault();
				fuelExpense.CategoryN = "";
				if (fuelExpenseData != null)
				{
					fuelExpense.CategoryN = fuelExpenseData.CategoryN;
				}
				basic.FuelExpenses.Add(fuelExpense);
			}
			if (basic.FuelExpense8 != null)
			{
				fuelExpense = new ExpenseCategoryViewModel();
				// set expenseC
				var expenseC = basic.FuelExpense8;
				fuelExpense.CategoryC = expenseC;
				// set expenseN
				var fuelExpenseData = _expenseCategoryRepository.Query(exp => exp.CategoryC == expenseC).FirstOrDefault();
				fuelExpense.CategoryN = "";
				if (fuelExpenseData != null)
				{
					fuelExpense.CategoryN = fuelExpenseData.CategoryN;
				}
				basic.FuelExpenses.Add(fuelExpense);
			}
			if (basic.FuelExpense9 != null)
			{
				fuelExpense = new ExpenseCategoryViewModel();
				// set expenseC
				var expenseC = basic.FuelExpense9;
				fuelExpense.CategoryC = expenseC;
				// set expenseN
				var fuelExpenseData = _expenseCategoryRepository.Query(exp => exp.CategoryC == expenseC).FirstOrDefault();
				fuelExpense.CategoryN = "";
				if (fuelExpenseData != null)
				{
					fuelExpense.CategoryN = fuelExpenseData.CategoryN;
				}
				basic.FuelExpenses.Add(fuelExpense);
			}
			if (basic.FuelExpense10 != null)
			{
				fuelExpense = new ExpenseCategoryViewModel();
				// set expenseC
				var expenseC = basic.FuelExpense10;
				fuelExpense.CategoryC = expenseC;
				// set expenseN
				var fuelExpenseData = _expenseCategoryRepository.Query(exp => exp.CategoryC == expenseC).FirstOrDefault();
				fuelExpense.CategoryN = "";
				if (fuelExpenseData != null)
				{
					fuelExpense.CategoryN = fuelExpenseData.CategoryN;
				}
				basic.FuelExpenses.Add(fuelExpense);
			}
			#endregion
			
			var licenseInfo = _licenseValidation.GetLicenseInfo(licensePath);
			result[0].LicenseCustomerName = licenseInfo.LicenseCustomerName;
			result[0].LicenseTruckLimitation = licenseInfo.LicenseTruckLimitation;
			result[0].LatestBackupDate = _backupRestoreDbService.GetLastestBackupDate(backupInfoPath);

			return result;
		}

		public List<ExpenseViewModel> GetExpenseCodesFromBasicSetting(string categoryI, string dispatchI)
		{
			var source = _basicRepository.GetAll().ToList();
			var destination = Mapper.Map<IEnumerable<Basic_S>, IEnumerable<BasicViewModel>>(source);
			var result = destination.ToList();

			if (result.Count <= 0) return null;
			var basic = result[0];
			if (categoryI == Convert.ToInt32(ExpenseCategory.Expense).ToString() && dispatchI.Equals("0"))
			{
				return GetExpensesCodeToList(basic);
			}
			else if (categoryI == Convert.ToInt32(ExpenseCategory.Surcharge).ToString() && dispatchI.Equals("0"))
			{
				return GetSurchargesCodeToList(basic);
			}
			else if (categoryI == Convert.ToInt32(ExpenseCategory.Allowance).ToString())
			{
				return GetAllowancesCodeToList(basic);
			}
			else if (categoryI == Convert.ToInt32(ExpenseCategory.Expense).ToString() && !dispatchI.Equals("0"))
			{
				return GetPartnerCostsCodeToList(basic);
			}
			else if (categoryI == Convert.ToInt32(ExpenseCategory.Surcharge).ToString() && !dispatchI.Equals("0"))
			{
				return GetPartnerSurchargesCodeToList(basic);
			}
			return null;
		}

		public List<ExpenseViewModel> GetExpensesCodeToList(BasicViewModel setting)
		{
			var expenses = new List<ExpenseViewModel>();
			if (setting.Expense1 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Expense1 });
			}
			if (setting.Expense2 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Expense2 });
			}
			if (setting.Expense3 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Expense3 });
			}
			if (setting.Expense4 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Expense4 });
			}
			if (setting.Expense5 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Expense5 });
			}
			if (setting.Expense6 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Expense6 });
			}
			if (setting.Expense7 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Expense7 });
			}
			if (setting.Expense8 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Expense8 });
			}
			if (setting.Expense9 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Expense9 });
			}
			if (setting.Expense10 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Expense10 });
			}
			return expenses.Count > 0 ? expenses : null;
		}
		public List<ExpenseViewModel> GetSurchargesCodeToList(BasicViewModel setting)
		{
			var expenses = new List<ExpenseViewModel>();
			if (setting.Surcharge1 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Surcharge1 });
			}
			if (setting.Surcharge2 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Surcharge2 });
			}
			if (setting.Surcharge3 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Surcharge3 });
			}
			if (setting.Surcharge4 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Surcharge4 });
			}
			if (setting.Surcharge5 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Surcharge5 });
			}
			if (setting.Surcharge6 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Surcharge6 });
			}
			if (setting.Surcharge7 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Surcharge7 });
			}
			if (setting.Surcharge8 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Surcharge8 });
			}
			if (setting.Surcharge9 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Surcharge9 });
			}
			if (setting.Surcharge10 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Surcharge10 });
			}
			return expenses.Count > 0 ? expenses : null;
		}
		public List<ExpenseViewModel> GetAllowancesCodeToList(BasicViewModel setting)
		{
			var expenses = new List<ExpenseViewModel>();
			if (setting.Allowance1 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Allowance1 });
			}
			if (setting.Allowance2 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Allowance2 });
			}
			if (setting.Allowance3 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Allowance3 });
			}
			if (setting.Allowance4 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Allowance4 });
			}
			if (setting.Allowance5 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Allowance5 });
			}
			if (setting.Allowance6 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Allowance6 });
			}
			if (setting.Allowance7 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Allowance7 });
			}
			if (setting.Allowance8 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Allowance8 });
			}
			if (setting.Allowance9 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Allowance9 });
			}
			if (setting.Allowance10 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.Allowance10 });
			}
			return expenses.Count > 0 ? expenses : null;
		}
		public List<ExpenseViewModel> GetPartnerCostsCodeToList(BasicViewModel setting)
		{
			var expenses = new List<ExpenseViewModel>();
			if (setting.PartnerCost1 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerCost1 });
			}
			if (setting.PartnerCost2 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerCost2 });
			}
			if (setting.PartnerCost3 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerCost3 });
			}
			if (setting.PartnerCost4 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerCost4 });
			}
			if (setting.PartnerCost5 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerCost5 });
			}
			if (setting.PartnerCost6 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerCost6 });
			}
			if (setting.PartnerCost7 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerCost7 });
			}
			if (setting.PartnerCost8 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerCost8 });
			}
			if (setting.PartnerCost9 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerCost9 });
			}
			if (setting.PartnerCost10 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerCost10 });
			}
			return expenses.Count > 0 ? expenses : null;
		}
		public List<ExpenseViewModel> GetPartnerSurchargesCodeToList(BasicViewModel setting)
		{
			var expenses = new List<ExpenseViewModel>();
			if (setting.PartnerSurcharge1 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerSurcharge1 });
			}
			if (setting.PartnerSurcharge2 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerSurcharge2 });
			}
			if (setting.PartnerSurcharge3 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerSurcharge3 });
			}
			if (setting.PartnerSurcharge4 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerSurcharge4 });
			}
			if (setting.PartnerSurcharge5 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerSurcharge5 });
			}
			if (setting.PartnerSurcharge6 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerSurcharge6 });
			}
			if (setting.PartnerSurcharge7 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerSurcharge7 });
			}
			if (setting.PartnerSurcharge8 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerSurcharge8 });
			}
			if (setting.PartnerSurcharge9 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerSurcharge9 });
			}
			if (setting.PartnerSurcharge10 != null)
			{
				expenses.Add(new ExpenseViewModel() { ExpenseC = setting.PartnerSurcharge10 });
			}
			return expenses.Count > 0 ? expenses : null;
		}

		public void ResetExpense(BasicViewModel basicsettingViewModel)
		{
			basicsettingViewModel.Expense1 = null;
			basicsettingViewModel.Expense2 = null;
			basicsettingViewModel.Expense3 = null;
			basicsettingViewModel.Expense4 = null;
			basicsettingViewModel.Expense5 = null;
			basicsettingViewModel.Expense6 = null;
			basicsettingViewModel.Expense7 = null;
			basicsettingViewModel.Expense8 = null;
			basicsettingViewModel.Expense9 = null;
			basicsettingViewModel.Expense10 = null;

			basicsettingViewModel.Surcharge1 = null;
			basicsettingViewModel.Surcharge2 = null;
			basicsettingViewModel.Surcharge3 = null;
			basicsettingViewModel.Surcharge4 = null;
			basicsettingViewModel.Surcharge5 = null;
			basicsettingViewModel.Surcharge6 = null;
			basicsettingViewModel.Surcharge7 = null;
			basicsettingViewModel.Surcharge8 = null;
			basicsettingViewModel.Surcharge9 = null;
			basicsettingViewModel.Surcharge10 = null;

			basicsettingViewModel.Allowance1 = null;
			basicsettingViewModel.Allowance2 = null;
			basicsettingViewModel.Allowance3 = null;
			basicsettingViewModel.Allowance4 = null;
			basicsettingViewModel.Allowance5 = null;
			basicsettingViewModel.Allowance6 = null;
			basicsettingViewModel.Allowance7 = null;
			basicsettingViewModel.Allowance8 = null;
			basicsettingViewModel.Allowance9 = null;
			basicsettingViewModel.Allowance10 = null;

			basicsettingViewModel.PartnerCost1 = null;
			basicsettingViewModel.PartnerCost2 = null;
			basicsettingViewModel.PartnerCost3 = null;
			basicsettingViewModel.PartnerCost4 = null;
			basicsettingViewModel.PartnerCost5 = null;
			basicsettingViewModel.PartnerCost6 = null;
			basicsettingViewModel.PartnerCost7 = null;
			basicsettingViewModel.PartnerCost8 = null;
			basicsettingViewModel.PartnerCost9 = null;
			basicsettingViewModel.PartnerCost10 = null;

			basicsettingViewModel.PartnerSurcharge1 = null;
			basicsettingViewModel.PartnerSurcharge2 = null;
			basicsettingViewModel.PartnerSurcharge3 = null;
			basicsettingViewModel.PartnerSurcharge4 = null;
			basicsettingViewModel.PartnerSurcharge5 = null;
			basicsettingViewModel.PartnerSurcharge6 = null;
			basicsettingViewModel.PartnerSurcharge7 = null;
			basicsettingViewModel.PartnerSurcharge8 = null;
			basicsettingViewModel.PartnerSurcharge9 = null;
			basicsettingViewModel.PartnerSurcharge10 = null;

			basicsettingViewModel.FuelExpense1 = null;
			basicsettingViewModel.FuelExpense2 = null;
			basicsettingViewModel.FuelExpense3 = null;
			basicsettingViewModel.FuelExpense4 = null;
			basicsettingViewModel.FuelExpense5 = null;
			basicsettingViewModel.FuelExpense6 = null;
			basicsettingViewModel.FuelExpense7 = null;
			basicsettingViewModel.FuelExpense8 = null;
			basicsettingViewModel.FuelExpense9 = null;
			basicsettingViewModel.FuelExpense10 = null;
		}

		public string GetDispatchTransportedColor()
		{
			var source = _basicRepository.GetAll().FirstOrDefault();
			return source != null ? source.StatusColor3 : "";
		}
        public string GetContainerStatus(DateTime orderD, string orderNo, int detailNo)
		{
			string status = null;
			var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
			var listDispatch =
				_dispatchRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo).ToList();
			if (listDispatch.Count <= 0)
			{
				status = basic != null ? basic.ContainerStatus + "-" + basic.DisplaySalary : "";
			}
			else
			{
				status = basic != null ? "-" + basic.DisplaySalary : "";
			}
			return status;
		}	
	
}

