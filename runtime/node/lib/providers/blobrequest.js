// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var azure = require('azure-storage'),
	StorageUtils = require('./storageutils');
	
var TableRequest = (function() {
	function TableRequest(account, params) {
		this.params = params;
		this.blobSvc = azure.createBlobService(account.name, account.key);
	}

	TableRequest.prototype.execute = function(callback) {
		var sharedAccessPolicy = {
			AccessPolicy: { 
				Permissions: this.params.permissions, 
				Expiry: StorageUtils.formatDate(this.params.expiry)
			}
		};	
			
		var relativePath = '/' + this.params.container + '/' + this.params.name;
		
		var resourceUrlSAS = this.createResourceURLWithSAS(relativePath, sharedAccessPolicy);
		var token = { "uri" : resourceUrlSAS };
		callback(null, token);
	};
	
	TableRequest.prototype.createResourceURLWithSAS = function(relativePath, sharedAccessPolicy) {
		// Generate the SAS for table
		var sasQueryString = this.blobSvc.generateSharedAccessSignature(this.params.container, this.params.name, sharedAccessPolicy);

		// Full path for resource with SAS
		return  'https://' + this.params.host + relativePath + '?' + sasQueryString;
	}; 	

	return TableRequest;
})();

module.exports = exports = TableRequest;