// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var proxyquire =  require('proxyquire').noPreserveCache();

describe('Broker', function() {
    var StorageUtils;

    beforeEach(function(){       
        StorageUtils = proxyquire('../lib/providers/storageutils', {
        });      
    });
	
    describe('readPermissions', function() {        
        it('reads permission by type', function() {
			['table', 'blob'].forEach(function(type) {
				testPermission(type, 'r', {read: true, write: false});
				testPermission(type, 'w', {read: false, write: true});
				testPermission(type, 'rw', {read: true, write: true});
				testPermission(type, 'wr', {read: true, write: true});
			});
        });	
		
		it('defaults permission to w', function() {
			testPermission('table', '', {read: false, write: true});
			testPermission('blob', '', {read: false, write: true});
        });	
		
		function testPermission(type, setting, expectedPermissions) {
			var config = {};
			var name = type[0].toUpperCase() + type.substring(1);
			config['ResourceBroker' + name + 'StoragePermissions'] = setting;
			var permissions = StorageUtils.readPermissions(type, config);
			expect(permissions.read).toEqual(expectedPermissions.read);
			expect(permissions.write).toEqual(expectedPermissions.write);
		}
	});
});