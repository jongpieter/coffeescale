var config = require('./config');
var azure = require('azure');

function EventPublisher() {
	this.serviceBusClient = azure.createServiceBusService(config.serviceBus.connectionString);	
}

EventPublisher.prototype.publish = function (label, data) {
	var message = {
		brokerProperties : { Label: label },
		body : JSON.stringify(data)
	};

	this.serviceBusClient.sendTopicMessage(config.serviceBus.topic, message, function(error) {
		if(error)
			console.log(error);
	});
}

exports.EventPublisher = EventPublisher;