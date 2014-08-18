// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var BlobRequest = require('./blobrequest'),
	Parameters = require('./parameters'),
	Permissions = require('./permissions'),	
	StorageUtils = require('./storageutils'),
	azure = require('azure-storage');

var BlobProvider = (function() {

    function BlobProvider(config) {				
		this.account = StorageUtils.readStorageAccount('blob', config);		
		this.allowedPermisions = StorageUtils.readPermissions('blob', config);		
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
		if(params.permissions.read && this.allowedPermisions.read) {
			permissions = azure.BlobUtilities.SharedAccessPermissions.READ;
		} 
		if (params.permissions.write && this.allowedPermisions.write) {
			permissions += azure.BlobUtilities.SharedAccessPermissions.WRITE;
		}		
		params.permissions = permissions;
		
		return new BlobRequest(this.account, params);
	};

    return BlobProvider;
})();

module.exports = exports = BlobProvider;