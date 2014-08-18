// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var Permissions = (function() {
    function Permissions() {				
		var read = false, 
			add = false, 
			update = false, 
			del = false;
		
		Object.defineProperty(this, 'read', {
			get: function() { return read; },
			set: function(value) { read = !!value; }
		});
		
		Object.defineProperty(this, 'add', {
			get: function() { return add; },
			set: function(value) { add = !!value; }
		});
		
		Object.defineProperty(this, 'update', {
			get: function() { return update; },
			set: function(value) { update = !!value; }
		});
		
		Object.defineProperty(this, 'del', {
			get: function() { return del; },
			set: function(value) { del = !!value; }
		});
		
		Object.defineProperty(this, 'write', {
			get: function() { return add && update && del; },
			set: function(value) { add = update = del = !!value; }
		});
		
		Object.defineProperty(this, 'readWrite', {
			get: function() { return this.read && this.write; },
			set: function(value) { this.read = this.write = !!value; }
		});
    }	
	
	Permissions.parse = function(value) {
		var permissions = new Permissions();
		
		if(value == 'r') {
			permissions.read = true;
		}
		else if (value == 'w') {
			permissions.write = true;
		}
		else if (value == 'rw' || value == 'wr') {
			permissions.readWrite = true;
		}
		
		return permissions;
	};

    return Permissions;
})();

module.exports = exports = Permissions;