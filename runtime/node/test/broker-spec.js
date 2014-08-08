// ----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ----------------------------------------------------------------------------

var proxyquire =  require('proxyquire').noPreserveCache();

describe('Broker', function() {
    var broker;

    function setup(providers) {        
        var Broker = proxyquire('../lib/broker', {
        });
        broker = new Broker(providers);             
    }
	
    describe('parseRequest', function() {        
        it('returns error request when type is not registered', function(done) {
			setup([]);
			var resourceReq = broker.parseRequest('table', {});
			resourceReq.execute(function(err, token) {
				expect(err).toEqual('type parameter is invalid');
				done();
			});
        });
		
		it('returns error request when type is missing', function(done) {
			setup([]);
			var resourceReq = broker.parseRequest(null, {});
			resourceReq.execute(function(err, token) {
				expect(err).toEqual('type parameter is missing');
				done();
			});
        });
	});
});