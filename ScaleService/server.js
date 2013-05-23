var scale = require('./d5scale');
var eventPublisher = require('./eventPublisher');
var intermediateQueue = require('./intermediateQueue');

var deviceController = new scale.DeviceController();
var queue = new intermediateQueue.Queue();
var publisher = new eventPublisher.EventPublisher();

queue.listen('dataChanged');

deviceController.on('dataChanged', function (e) {		
	console.log("Device data was changed: ");
	console.log(e);	
	queue.enqueue('dataChanged', e);
});

queue.on('dataChanged', function (e) {	

	console.log("Queue emitted dataChanged event:");
	console.log(e);

	publisher.publish('dataChanged', e, function(err) { 
		if(err)
		{
			console.log("Publish error, retrying:");
			console.log(e);
			queue.retry('dataChanged', e);
		}
		else						
		{
			console.log("Publish success, completing:");
			console.log(e);
			queue.complete('dataChanged', e);		
		}
	});
});