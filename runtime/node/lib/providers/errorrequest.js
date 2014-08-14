// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var ErrorRequest = (function() {
    function ErrorRequest(message) {
		this.message = message;
    }
	
	ErrorRequest.prototype.execute = function(callback) {
		callback(this.message);
	};

    return ErrorRequest;
})();

module.exports = exports = ErrorRequest;