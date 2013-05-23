var util = require('util');
var config = require('./config');
var events = require('events');
var redis = require("redis");

function Queue() {	

	this.enqueueClient = redis.createClient();
	this.dequeueClient = redis.createClient();

	this.enqueueClient.on("error", function (err) {
		console.log("Error " + err);
	});

	this.dequeueClient.on("error", function (err) {
		console.log("Error " + err);
	});	
}

util.inherits(Queue, events.EventEmitter);

Queue.prototype.enqueue = function (event, data) {	
	this.enqueueClient.lpush(event, JSON.stringify(data));
}

Queue.prototype.listen = function(event) {
	var self = this;

	this.dequeueClient.brpoplpush(event, event + '_inprogress', 0, function(err, reply) {		
		self.emit(event, JSON.parse(reply));		
	});
}

Queue.prototype.complete = function(event, data) {	
	this.dequeueClient.lrem(event + '_inprogress', 0, JSON.stringify(data));
	this.listen(event);
}

Queue.prototype.retry = function(event, data) {	
	
	this.enqueueClient.multi()
		.lrem(event + '_inprogress', 0, JSON.stringify(data))
		.rpush(event, JSON.stringify(data))
		.exec();

	this.listen(event);
}

exports.Queue = Queue;