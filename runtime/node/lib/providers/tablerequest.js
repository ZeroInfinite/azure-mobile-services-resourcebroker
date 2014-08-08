// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var azure = require('azure-storage');
	
var TableRequest = (function() {
	function TableRequest(account, params) {
		this.params = params;
		this.tableSvc = azure.createTableService(account.name, account.key);
	}

	TableRequest.prototype.execute = function(callback) {
		var sharedAccessPolicy = {
			AccessPolicy: { 
				Permissions: this.params.permissions, 
				Expiry: formatDate(this.params.expiry)
			}
		};	
			
		var relativePath = '/' + this.params.name;
		
		var resourceUrlSAS = this.createResourceURLWithSAS(relativePath, sharedAccessPolicy);
		var token = { "uri" : resourceUrlSAS };
		callback(null, token);
	};
	
	TableRequest.prototype.createResourceURLWithSAS = function(relativePath, sharedAccessPolicy) {
		// Generate the SAS for table
		var sasQueryString = this.tableSvc.generateSharedAccessSignature(this.params.name, sharedAccessPolicy);

		// Full path for resource with SAS
		return  'https://' + this.params.host + relativePath + '?' + sasQueryString;
	}; 
	
	function formatDate(date) { 
		var raw = date.toJSON(); 
		// storage service does not like milliseconds on the end of the time so strip 
		return raw.substr(0, raw.lastIndexOf('.')) + 'Z'; 
	}

	return TableRequest;
})();

module.exports = exports = TableRequest;