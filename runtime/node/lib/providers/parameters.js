// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var Permissions = require('./permissions');

var Parameters = (function() {
    function Parameters() {
    }
	
	Parameters.parse = function(data) {
		validateInputs(data);		
		
		var permissions = new Permissions();
		
		if(data.permissions == 'r') {
			permissions.read = true;
		}
		else if (data.permissions == 'w') {
			permissions.write = true;
		}
		else if (data.permissions == 'rw' || data.permissions == 'wr') {
			permissions.readWrite = true;
		}
		
		var params = {
			name: data.name,
			permissions: permissions,
			expiry: data.expiry
		};
		
		return params;
	};
	
	function validateInputs(data)
	{	
		// Validate existence of body parameters
		validateParamNotNull(data, 'name');
		validateParamNotNull(data, 'permissions');
		validateParamNotNull(data, 'expiry');

		// Validate permissions string
		if(!(data.permissions == "r" || 
			 data.permissions == "w" || 
			 data.permissions == "rw" || 
			 data.permissions == "wr"))
		{
			throw new Error("permissions is invalid value");
		}

		// Validate expiry
		data.expiry = new Date(Date.parse(data.expiry));
		if( Object.prototype.toString.call(data.expiry) != "[object Date]" || 
			isNaN(data.expiry.getTime()))
		{
			throw new Error("expiry is invalid Date object");
		}		
	}
	
	function validateParamNotNull(data, name) {
		if (!data[name]) {
			throw new Error(name + " parameter is missing");
		}
	}

    return Parameters;
})();

module.exports = exports = Parameters;