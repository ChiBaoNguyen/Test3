using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using Root.Data.Infrastructure;
using System;
using System.Linq;
using System.Collections.Generic;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.Operation;

namespace Service.Services
{
	public interface IOperationService
	{
		IEnumerable<OperationViewModel> Get(string orderTypeI);
		IEnumerable<OperationViewModel> Get();
		IEnumerable<OperationViewModel> GetForSuggestion(string value, string orderTypeI);
		OperationViewModel GetByName(string name, string orderTypeI);
		void Update(List<OperationViewModel> newOperations);
	}
	public class OperationService : IOperationService
	{
		private readonly IOperationRepository _operationRepository;
		private readonly IUnitOfWork _unitOfWork;
		public OperationService(IOperationRepository operationRepository, IUnitOfWork unitOfWork)
		{
			_operationRepository = operationRepository;
			_unitOfWork = unitOfWork;
		}
		public IEnumerable<OperationViewModel> Get(string orderTypeI)
		{
			var operations = _operationRepository.GetAll().Where(x => x.OrderTypeI.Equals(orderTypeI)).OrderBy(x => x.DisplayLineNo);

			if (operations.Any())
			{
				var destination = Mapper.Map<IEnumerable<Operation_M>, IEnumerable<OperationViewModel>>(operations);
				return destination;
			}
			return null;
		}

		public IEnumerable<OperationViewModel> Get()
		{
			var operations = _operationRepository.GetAll().OrderBy(x => x.DisplayLineNo);

			if (operations.Any())
			{
				var destination = Mapper.Map<IEnumerable<Operation_M>, IEnumerable<OperationViewModel>>(operations);
				return destination;
			}
			return null;
		}

		public void Update(List<OperationViewModel> newOperations)
		{
			var operations = _operationRepository.GetAll();
			var maxItemCode = 0;
			//if (operations.Any())
			//{
			//	maxItemCode = operations.Max(x => x.OperationC);
			//}
			//xoa
			//foreach (var item in operations)
			//{
			//	if (newOperations.Any(x => x.OperationC == item.OperationC && x.OrderTypeI == item.OrderTypeI) == false)
			//	{
			//		_operationRepository.Delete(item);
			//	}
			//}
			//update
			foreach (var item in newOperations)
			{
				//if (item.OperationC == 0)
				//{
				//	var addItem = new Operation_M()
				//	{
				//		DisplayLineNo = item.DisplayLineNo,
				//		OperationC = ++maxItemCode,
				//		OperationN = item.OperationN,
				//		Description = item.Description,
				//		OrderTypeI = item.OrderTypeI
				//	};
				//	_operationRepository.Add(addItem);
				//}
				//else
				//{
					var updateMaintenance = operations.FirstOrDefault(x => x.OperationC == item.OperationC);
					if (updateMaintenance != null)
					{
						//updateMaintenance.DisplayLineNo = item.DisplayLineNo;
						updateMaintenance.OperationN = item.OperationN;
						//updateMaintenance.Description = item.Description;
						//updateMaintenance.OrderTypeI = item.OrderTypeI;

						_operationRepository.Update(updateMaintenance);
					}
				//}
			}
			SaveOperation();
		}
		public void SaveOperation()
		{
			_unitOfWork.Commit();
		}


		public IEnumerable<OperationViewModel> GetForSuggestion(string value, string orderTypeI)
		{
			if (value == null) return null;

			var operations = _operationRepository
				.GetAll()
				.Where(x => (x.OperationN.Contains(value) || x.OperationC.ToString().Contains(value)) && x.OrderTypeI == orderTypeI)
				.OrderBy(x => x.DisplayLineNo);

			if (operations.Any())
			{
				var destination = Mapper.Map<IEnumerable<Operation_M>, IEnumerable<OperationViewModel>>(operations);
				return destination;
			}
			return null;
		}


		public OperationViewModel GetByName(string name, string orderTypeI)
		{
			var operations = _operationRepository.Query(x => x.OperationN == name && x.OrderTypeI == orderTypeI)
				.OrderBy(x => x.DisplayLineNo).FirstOrDefault();
			if (operations != null)
			{
				var destination = Mapper.Map<Operation_M, OperationViewModel>(operations);
				return destination;
			}
			return null;
		}
	}
}
