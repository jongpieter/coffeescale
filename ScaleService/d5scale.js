var util = require('util');
var HID = require('node-hid');
var events = require('events');

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
		hid.read(this.deviceData.bind(this, { hid: hid, info: devices[i], oldData: "" }));
	}
}

util.inherits(DeviceController, events.EventEmitter);

DeviceController.prototype.deviceData = function (device, error, data) {

	if(error)
	{
		console.log(error);		
		return;
	}
		
	if(device.oldData.toString() !== data.toString())
	{
		device.oldData = data.toString();
		this.emit('dataChanged', { device : { serialNumber: device.info.serialNumber, manufacturer: device.info.manufacturer, product: device.info.product }, data : data });
	}
	
    device.hid.read(this.deviceData.bind(this, device));
}

exports.DeviceController = DeviceController;