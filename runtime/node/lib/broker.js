// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var ErrorRequest = require('./providers/errorrequest');

var Broker = (function() {
    function Broker(providers) {
		var self = this;
		
		self.providers = { };
		providers = providers || [];
		
		providers.forEach(function(provider) {
			self.providers[provider.type] = provider;
		});
    }
	
	Broker.prototype.parseRequest = function(type, params) {
		if (!type) {
			return new ErrorRequest('type parameter is missing');
		}
		var provider = this.providers[type];
		if (provider) {
			try {
				return provider.parseRequest(params);
			}
			catch (e) {
				return new ErrorRequest(e.stack);
			}
		}
		else {
			return new ErrorRequest('type parameter is invalid');
		}
	};
	
	Broker.prototype.executeRequest = function (resourceReq, res) {
		resourceReq.execute(function(err, token) {
			if (err) {
				res.send(400, err);
				return;
			}
			res.send(200, token);
		});
	};	

    return Broker;
})();

module.exports = exports = Broker;