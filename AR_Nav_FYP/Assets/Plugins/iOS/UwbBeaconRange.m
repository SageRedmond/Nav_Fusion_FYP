#import "UwbBeaconRange.h"

id<SendsBeaconRange> beaconRangeSender;

void RegisterBeaconRangeSender(id<SendsBeaconRange>  sender){
    beaconRangeSender = sender
}

void OnSendBeaconRange(BeaconRangeCallback callback){
    beaconRangeSender.sendBeaconRange = callback
}