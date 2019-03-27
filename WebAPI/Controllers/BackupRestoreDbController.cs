using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;

namespace WebAPI.Controllers
{
    public class BackupRestoreDbController : ApiController
    {
	    public IBackupRestoreDbService _backupRestoreDbService;
		public BackupRestoreDbController() { }
		public BackupRestoreDbController(IBackupRestoreDbService backupRestoreDbService)
		{
			this._backupRestoreDbService = backupRestoreDbService;
		}

		[Filters.Authorize(Roles = "RestoreBackupDb")]
	    public IHttpActionResult Get(string description)
	    {
			var sqlInstanceName = System.Configuration.ConfigurationManager.AppSettings["SqlInstanceName"];
			var bkpDbName = System.Configuration.ConfigurationManager.AppSettings["BackupDBName"];
			var sqlUsername = System.Configuration.ConfigurationManager.AppSettings["SqlUserName"];
			var sqlPassword = System.Configuration.ConfigurationManager.AppSettings["SqlPassword"];
			var bkpPath = System.Configuration.ConfigurationManager.AppSettings["BackupPath"];
			var backupStatus = _backupRestoreDbService.BackupDb(description, bkpPath, sqlInstanceName, bkpDbName, sqlUsername, sqlPassword);
			return Ok(backupStatus);
	    }

		[Filters.Authorize(Roles = "RestoreBackupDb")]
	    [Route("api/BackupRestoreDb/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "BackupD",
				  bool reverse = false,
				  string search = null)
		{
			var bkpPath = System.Configuration.ConfigurationManager.AppSettings["BackupPath"];
			var custDatatable = _backupRestoreDbService.GetRestoreDataTable(page, itemsPerPage, sortBy, reverse, search, bkpPath + "Info.txt");
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		[HttpGet]
		[Filters.Authorize(Roles = "RestoreBackupDb")]
		[Route("api/BackupRestoreDb/RestoreDb")]
		public IHttpActionResult RestoreDb(string bkpFileName)
	    {
			var sqlInstanceName = System.Configuration.ConfigurationManager.AppSettings["SqlInstanceName"];
			var bkpDbName = System.Configuration.ConfigurationManager.AppSettings["BackupDBName"];
			var sqlUsername = System.Configuration.ConfigurationManager.AppSettings["SqlUserName"];
			var sqlPassword = System.Configuration.ConfigurationManager.AppSettings["SqlPassword"];
			var bkpPath = System.Configuration.ConfigurationManager.AppSettings["BackupPath"];
			var restoreStatus =
				_backupRestoreDbService.RestoreDb(bkpPath, bkpFileName, sqlInstanceName, bkpDbName, sqlUsername, sqlPassword);

			return Ok(restoreStatus);
	    }
    }
}
