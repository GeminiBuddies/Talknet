using System;

namespace Talknet {
    public partial class CommandInvoker {
        public void Register(string command, Func<int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0>(string command, Func<T0, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1>(string command, Func<T0, T1, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2>(string command, Func<T0, T1, T2, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3>(string command, Func<T0, T1, T2, T3, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3, T4>(string command, Func<T0, T1, T2, T3, T4, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3, T4, T5>(string command, Func<T0, T1, T2, T3, T4, T5, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3, T4, T5, T6>(string command, Func<T0, T1, T2, T3, T4, T5, T6, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3, T4, T5, T6, T7>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);
        public void Register<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, int> handler, params string[] alternativeForms) => Register(command, (Delegate)handler, alternativeForms);

        public void Update(string command, Func<int> handler) => Update(command, (Delegate)handler);
        public void Update<T0>(string command, Func<T0, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1>(string command, Func<T0, T1, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2>(string command, Func<T0, T1, T2, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3>(string command, Func<T0, T1, T2, T3, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3, T4>(string command, Func<T0, T1, T2, T3, T4, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3, T4, T5>(string command, Func<T0, T1, T2, T3, T4, T5, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3, T4, T5, T6>(string command, Func<T0, T1, T2, T3, T4, T5, T6, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3, T4, T5, T6, T7>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, int> handler) => Update(command, (Delegate)handler);
        public void Update<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, int> handler) => Update(command, (Delegate)handler);

        public void RegisterOrUpdate(string command, Func<int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0>(string command, Func<T0, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1>(string command, Func<T0, T1, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2>(string command, Func<T0, T1, T2, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3>(string command, Func<T0, T1, T2, T3, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3, T4>(string command, Func<T0, T1, T2, T3, T4, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3, T4, T5>(string command, Func<T0, T1, T2, T3, T4, T5, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3, T4, T5, T6>(string command, Func<T0, T1, T2, T3, T4, T5, T6, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3, T4, T5, T6, T7>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
        public void RegisterOrUpdate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF>(string command, Func<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TA, TB, TC, TD, TE, TF, int> handler) => RegisterOrUpdate(command, (Delegate)handler);
    }
}
