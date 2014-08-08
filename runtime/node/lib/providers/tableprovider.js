// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var Parameters = require('./parameters'),
	TableRequest = require('./tablerequest'),
	azure = require('azure-storage');

var TableProvider = (function() {
    function TableProvider(config) {				
		this.account = {
			name: config.ResourceBrokerTableStorageAccountName || config.ResourceBrokerStorageAccountName,
			key: config.ResourceBrokerTableStorageAccountKey || config.ResourceBrokerStorageAccountKey
		};
		
		if (!this.account.name) {
			throw new Error('Either the ResourceBrokerTableStorageAccountName or ResourceBrokerStorageAccountName setting must be defined');
		}
		
		if (!this.account.key) {
			throw new Error('Either the ResourceBrokerTableStorageAccountKey or ResourceBrokerStorageAccountKey setting must be defined');
		}
		
		this.type = 'table';
    }
	
	TableProvider.prototype.parseRequest = function(data) {
		var params = Parameters.parse(data);
		params.host = this.account.name + '.table.core.windows.net';
		
		var permissions = '';
		if(params.permissions.read) {
			permissions = azure.TableUtilities.SharedAccessPermissions.QUERY;
		} 
		if (params.permissions.write) {
			permissions += azure.TableUtilities.SharedAccessPermissions.ADD + azure.TableUtilities.SharedAccessPermissions.UPDATE + azure.TableUtilities.SharedAccessPermissions.DELETE;
		}
		params.permissions = permissions;
		
		return new TableRequest(this.account, params);
	};

    return TableProvider;
})();

module.exports = exports = TableProvider;