struct UwbBeaconRangeData
{
    const char* _Nonnull beaconId;
    const float distance;
};

typedef void (*BeaconRangeCallback)(struct UwbBeaconRangeData newRangeData);

// @protocol is Objective-C specific syntax, standerd c/c++ extension doesn't recognise it
@protocol SendsBeaconRange
/* Function pointer that will be used to send state from Swift to Unity.
   Encapsulation within a protocol lets us take advantage of Swift's didSet property observer. */
@property (nullable) BeaconRangeCallback sendBeaconRange;
@end

__attribute__ ((visibility("default")))
void RegisterBeaconRangeSender(id<SendsBeaconRange> _Nonnull sender);
