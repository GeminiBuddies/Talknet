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
 * a) I can handle it                             b) I cannot handle it
 * ↓                                              ↓
 * Logger.Error and return error code             just throw it, invoker will catch it and pack it in a "CommandExitAbnormallyException"
 * ↓ throw it                                     ↓
 * Main loop show information and continue        Main loop show information and exit
 */
