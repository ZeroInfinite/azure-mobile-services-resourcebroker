// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var Broker = require('./broker'),
	BlobProvider = require('./providers/blobprovider'),
	TableProvider = require('./providers/tableprovider');

exports.register = function (api, options) {
	if (!options) {
		throw new Error('options are missing');
	}
	if (!options.config) {
		throw new Error('config missing in options');
	}
	var providers = options.providers;
	if (providers && !Array.isArray(providers)) {
		providers = [providers];
	}
	else if (!providers) {
		providers = loadProviders(options.config);
	}
	
	var broker = new Broker(providers);
	
	api.post('/:type', function(req, res) {
		var resReq = broker.parseRequest(req.params.type, req.body);
		if (typeof options.callback === 'function') {
			options.callback(resReq, function(err) {
				if (err) {
					res.send(400, err);
					return;
				}				
				broker.executeRequest(resReq, res);
			});
		}
		else {
			broker.executeRequest(resReq, res);
		}
	});
	
	function loadProviders(config) {
		return [
			new BlobProvider(config),
			new TableProvider(config)
		];
	}
};