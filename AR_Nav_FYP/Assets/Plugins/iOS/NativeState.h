struct NativeState
{
    const float distance;
};

typedef void (*SetNativeStateCallback)(struct NativeState nextState);

// @protocol is Objective-C specific syntax, standerd c/c++ extension doesn't recognise it
@protocol SetsNativeState
/* Function pointer that will be used to send state from Swift to Unity.
   Encapsulation within a protocol lets us take advantage of Swift's didSet property observer. */
@property (nullable) SetNativeStateCallback setNativeState;
@end

__attribute__ ((visibility("default")))
void RegisterNativeStateSetter(id<SetsNativeState> _Nonnull setter);
