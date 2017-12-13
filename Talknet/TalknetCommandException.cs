using System;

/* Exception model here:
 * Main loop 
 * ↓ call
 * Invoker
 * ↓ call
 * Command handler
 * ↓ 
 * ...... (some codes)
 * ↓ oops, exception!
 * Command handlers catch this expection and ...
 * a) fatal exception, cannot continue;      b) a normal one;     c) failed to catch it or another exception while handling it
 * ↓                                         ↓                    ↓
 * Pack it into a TalknetCommandException    handle it            just throw it, invoker will catch it and pack it in a "CommandExitAbnormallyException"
 * ↓ throw it                                                     ↓
 * Main loop show information and continue                        Main loop show information and exit
 */

namespace Talknet {
    public class TalknetCommandException : Exception {
        // deleted
        // public TalknetCommandException() { }
        public TalknetCommandException(string message) : base(message) { }
        public TalknetCommandException(string message, Exception inner) : base(message, inner) { }
    }
}
