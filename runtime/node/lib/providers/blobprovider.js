// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var Parameters = require('./parameters'),
	BlobRequest = require('./blobrequest'),
	azure = require('azure-storage');

var BlobProvider = (function() {
    function BlobProvider(config) {				
		this.account = {
			name: config.ResourceBrokerBlobStorageAccountName || config.ResourceBrokerStorageAccountName,
			key: config.ResourceBrokerBlobStorageAccountKey || config.ResourceBrokerStorageAccountKey
		};
		
		if (!this.account.name) {
			throw new Error('Either the ResourceBrokerBlobStorageAccountName or ResourceBrokerStorageAccountName setting must be defined');
		}
		
		if (!this.account.key) {
			throw new Error('Either the ResourceBrokerBlobStorageAccountKey or ResourceBrokerStorageAccountKey setting must be defined');
		}
		
		this.type = 'blob';
    }
	
	BlobProvider.prototype.parseRequest = function(data) {
		var params = Parameters.parse(data);
		params.host = this.account.name + '.blob.core.windows.net';
		
		// Validate container
		if(!data.container)
		{
			throw new Error("container parameter is missing");
		}
		params.container = data.container;		
		
		var permissions = '';
		if(params.permissions.read) {
			permissions = azure.BlobUtilities.SharedAccessPermissions.READ;
		} 
		if (params.permissions.write) {
			permissions += azure.BlobUtilities.SharedAccessPermissions.WRITE;
		}		
		params.permissions = permissions;
		
		return new BlobRequest(this.account, params);
	};

    return BlobProvider;
})();

module.exports = exports = BlobProvider;