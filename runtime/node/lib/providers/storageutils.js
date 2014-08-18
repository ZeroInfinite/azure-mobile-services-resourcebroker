// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var Permissions = require('./permissions');

var StorageUtils = (function() {
	
	var defaultAllowedPermissions = { read: false, write: true };
	
	function StorageUtils() {
	}	
	
	StorageUtils.readPermissions = function (type, config) {
		type = getTypeName(type);
		var typePermissionName = 'ResourceBroker' + type + 'StoragePermissions';
		var permissions = config[typePermissionName] ? Permissions.parse(config[typePermissionName]): defaultAllowedPermissions;
		return permissions;
	};
	
	StorageUtils.readStorageAccount = function(type, config) {
		type = getTypeName(type);
		var typeAccountName = 'ResourceBroker' + type + 'StorageAccountName',
			typeAccountKey = 'ResourceBroker' + type + 'StorageAccountKey';
		
		var account = {
			name: config[typeAccountName] || config.ResourceBrokerStorageAccountName,
			key: config[typeAccountKey] || config.ResourceBrokerStorageAccountKey
		};		
		if (!account.name) {
			throw new Error('Either the ' + typeAccountName + ' or ResourceBrokerStorageAccountName setting must be defined');
		}		
		if (!account.key) {
			throw new Error('Either the ' + typeAccountKey + ' or ResourceBrokerStorageAccountKey setting must be defined');
		}
		
		return account;
	}
		
	StorageUtils.formatDate = function(date) { 
		var raw = date.toJSON(); 
		// storage service does not like milliseconds on the end of the time so strip 
		return raw.substr(0, raw.lastIndexOf('.')) + 'Z'; 
	};
	
	function getTypeName(type) {
		return type[0].toUpperCase() + type.substring(1).toLowerCase();
	}

	return StorageUtils;
})();

module.exports = exports = StorageUtils;