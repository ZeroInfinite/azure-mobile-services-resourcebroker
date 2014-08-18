// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var TableRequest = require('./tablerequest'),
	Parameters = require('./parameters'),
	Permissions = require('./permissions'),	
	StorageUtils = require('./storageutils'),
	azure = require('azure-storage');

var TableProvider = (function() {

    function TableProvider(config) {				
		this.account = StorageUtils.readStorageAccount('table', config);		
		this.allowedPermisions = StorageUtils.readPermissions('table', config);		
		this.type = 'table';
    }
	
	TableProvider.prototype.parseRequest = function(data) {
		var params = Parameters.parse(data);
		params.host = this.account.name + '.table.core.windows.net';
		
		var permissions = '';
		if(params.permissions.read && this.allowedPermisions.read) {
			permissions = azure.TableUtilities.SharedAccessPermissions.QUERY;
		} 
		if (params.permissions.write && this.allowedPermisions.write) {
			permissions += azure.TableUtilities.SharedAccessPermissions.ADD + azure.TableUtilities.SharedAccessPermissions.UPDATE + azure.TableUtilities.SharedAccessPermissions.DELETE;
		}
		params.permissions = permissions;
		
		return new TableRequest(this.account, params);
	};

    return TableProvider;
})();

module.exports = exports = TableProvider;