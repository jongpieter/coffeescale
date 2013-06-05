var util = require('util');
var HID = require('node-hid');
var events = require('events');
var CBuffer = require('CBuffer');

function DeviceController() {
	var devices = HID.devices(2338, 32773);	
	
    if (!devices.length) {
        throw new Error("No devices found");
    }	
	
	console.log("Devices found:");
	console.log(devices);
	
	events.EventEmitter.call(this);

	for(var i = 0; i < devices.length; i++)
	{
		var hid = new HID.HID(devices[i].path);
		var buffer = new CBuffer(3);		

		hid.read(this.deviceData.bind(this, { hid: hid, info: devices[i], oldWeights: buffer, lastSentWeight: -1 }));
	}
}

util.inherits(DeviceController, events.EventEmitter);

var getWeight = function(data) {

	var weight = data[4] + (data[5] * 256)
	if(data[2] == 11)
		return Math.round(weight * 28.3495 / 10);

	return weight;
} 

DeviceController.prototype.deviceData = function (device, error, data) {

	if(error)
		throw new Error(error);			

	var message = { 
		serialNumber: device.info.serialNumber,
		weight: getWeight(data),
		status: data[1],
		date: new Date()
	};

	if(device.oldWeights.every(function(val) { return Math.abs(message.weight - val) <= 2; }) && Math.abs(message.weight - device.lastSentWeight) > 2 )
	{		
		this.emit('dataChanged', message);
		device.lastSentWeight = message.weight;
	}

	device.oldWeights.push(message.weight);
    device.hid.read(this.deviceData.bind(this, device));
}

exports.DeviceController = DeviceController;