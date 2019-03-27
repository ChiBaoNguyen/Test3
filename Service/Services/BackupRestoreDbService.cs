using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using Website.ViewModels.BackupRestoreDB;

namespace Service.Services
{
	public interface IBackupRestoreDbService
	{
		RestoreDatatable GetRestoreDataTable(int page, int itemsPerPage, string sortBy, bool reverse,
			string value, string backupInfoPath);

		DateTime? GetLastestBackupDate(string backupInfoPath);

		object RestoreDb(string backupFolderPath, string bkpFileName,
						string instName, string bkpDbName, string uname, string pw);
		object BackupDb(string description, string backupFolderPath,
						string instName, string bkpDbName, string uname, string pw);
	}
	public class BackupRestoreDbService : IBackupRestoreDbService
	{
		public RestoreDatatable GetRestoreDataTable(int page, int itemsPerPage, string sortBy, bool reverse, string value, string backupInfoPath)
		{
			var restoreData = GetBackupInfo(backupInfoPath);
			// searching
			//if (!string.IsNullOrWhiteSpace(custSearchValue))
			//{
			//	custSearchValue = custSearchValue.ToLower();
			//	departments = departments.Where(cus => cus.DepN.ToLower().Contains(custSearchValue)
			//													|| cus.DepC.ToLower().Contains(custSearchValue)
			//													);
			//}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			//departments = departments.AsQueryable().OrderBy("\"" + sortBy + (reverse ? " desc\"" : " asc\"")).AsEnumerable();
			if (restoreData != null)
			{
				var restoreDataOrdered = restoreData.OrderBy(sortBy + (reverse ? " descending" : ""));

				// paging
				var restoreDataPaged = restoreDataOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

				//var destination = Mapper.Map<List<Department_M>, List<DepartmentViewModel>>(departmentsPaged);
				var custDatatable = new RestoreDatatable()
				{
					Data = restoreDataPaged,
					Total = restoreData.Count()
				};
				return custDatatable;
			}
			return null;
		}

		public DateTime? GetLastestBackupDate(string backupInfoPath)
		{
			var restoreData = GetBackupInfo(backupInfoPath);
			if (restoreData != null)
			{
				var restoreDataOrdered = restoreData.OrderBy("BackupD descending");
				var lastestBackup = restoreDataOrdered.FirstOrDefault();
				if (lastestBackup != null)
				{
					var lastestBackupDate = lastestBackup.BackupD;
					return lastestBackupDate;
				}
			}
			return null;
		}

		public object BackupDb(string description, string backupFolderPath,
								string instName, string bkpDbName, string uname, string pw)
		{
			var fileName = backupFolderPath + "Info.txt";
			try
			{
				bool exists = Directory.Exists(backupFolderPath);

				if (!exists)
				{
					Directory.CreateDirectory(backupFolderPath);
				}

				var bkpFileName = "Backup_" + DateTime.Now.ToString("yyyyMMddhhmmsstt") + ".bak";

				// Create a new file 
				using (var sw = new StreamWriter(fileName, true))
				{
					sw.WriteLine("{0};{1};{2}", DateTime.Now, bkpFileName, description);
				}

				//return Backup(instName, bkpDbName, uname, pw, backupFolderPath + "/" + bkpFileName);
				return Backup(instName, bkpDbName, uname, pw, backupFolderPath + bkpFileName);
			}
			catch (Exception ex)
			{
				return new { Error = 1, Message = ex };
			}
		}

		private object Backup(string instName, string bkpDbName, string uname, string pw, string bkpFileName)
		{
			try
			{
				var conn = new ServerConnection(instName, uname, pw);
				var srv = new Server(conn);

				var bkp = new Backup {Action = BackupActionType.Database, Database = bkpDbName, Incremental = false};
				bkp.Devices.AddDevice(bkpFileName, DeviceType.File);
				bkp.SqlBackup(srv);
				return new {Error = 0, Message = "Successfully"};

			}
			catch (Exception ex)
			{
				return new { Error = 1, Message = ex };
			}
		}

		public object RestoreDb(string backupFolderPath, string bkpFileName, string instName, 
								string bkpDbName, string uname, string pw)
		{
			return Restore(instName, bkpDbName, uname, pw, backupFolderPath + bkpFileName);
		}

		private object Restore(string instName, string bkpDbName, string uname, string pw, string bkpFileName)
		{
			try
			{
				var conn = new ServerConnection(instName, uname, pw);
				var srv = new Server(conn);

				var res = new Restore { Action = RestoreActionType.Database, Database = bkpDbName, ReplaceDatabase = true};
				res.Devices.AddDevice(bkpFileName, DeviceType.File);

				// Kill all processes
				srv.KillAllProcesses(res.Database);

				// Set single-user mode
				Database db = srv.Databases[res.Database];
				db.DatabaseOptions.UserAccess = DatabaseUserAccess.Single;
				db.Alter(TerminationClause.RollbackTransactionsImmediately);

				// Detach database
				srv.DetachDatabase(res.Database, false);

				res.SqlRestore(srv);
				return new { Error = 0, Message = "Successfully" };
			}
			catch (Exception ex)
			{
				return new { Error = 1, Message = ex };
			}
		}

		public IQueryable<RestoreDataViewModel> GetBackupInfo(string backupInfoPath)
		{
			var restoreDataList = new List<RestoreDataViewModel>();
			try
			{
				using (var file = new StreamReader(backupInfoPath))
				{
					string line;
					while ((line = file.ReadLine()) != null)
					{
						var lineInfo = line.Split(';');
						if (lineInfo.Any())
						{
							var backupDate = lineInfo[0];
							var bkpFileName = lineInfo[1];
							var backupDescription = lineInfo[2];
							var restoreData = new RestoreDataViewModel()
							{
								BackupD = DateTime.Parse(backupDate),
								BackupFileName = bkpFileName,
								Description = backupDescription,
							};

							restoreDataList.Add(restoreData);
						}
					}
				}
				return restoreDataList.AsQueryable();
			}
			catch (Exception)
			{
				return null;
			}
			
		}
	}
}
