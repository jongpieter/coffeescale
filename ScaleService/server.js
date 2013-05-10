var scale = require('./d5scale');
var eventPublisher = require('./eventPublisher');

var deviceController = new scale.DeviceController();
var publisher = new eventPublisher.EventPublisher();

deviceController.on('dataChanged', function (e) {
	publisher.publish('dataChanged', e);
    console.log(e);	
});