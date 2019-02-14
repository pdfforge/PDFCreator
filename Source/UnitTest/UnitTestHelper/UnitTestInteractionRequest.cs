using NUnit.Framework;
using pdfforge.Obsidian.Interaction;
using pdfforge.Obsidian.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UnitTest.UnitTestHelper
{
    public class UnitTestInteractionRequest : IInteractionRequest
    {
        private readonly Dictionary<Type, Action<IInteraction>> _registrations = new Dictionary<Type, Action<IInteraction>>();
        private readonly List<IInteraction> _raisedInteractions = new List<IInteraction>();

        public void RegisterInteractionHandler<T>(Action<T> action) where T : class, IInteraction
        {
            _registrations.Add(typeof(T), i => action(i as T));
        }

        public T AssertWasRaised<T>() where T : class, IInteraction
        {
            return AssertWasRaised<T>(i => true);
        }

        public T AssertWasRaised<T>(Predicate<T> interactionPredicate) where T : class, IInteraction
        {
            var interaction = _raisedInteractions.FirstOrDefault(i => i is T && interactionPredicate(i as T));
            if (interaction == null)
                Assert.Fail($"An interaction with type {typeof(T)} which fulfills the predicate was not raised");
            return interaction as T;
        }

        public void AssertWasNotRaised<T>() where T : class, IInteraction
        {
            AssertWasNotRaised<T>(i => true);
        }

        public void AssertWasNotRaised<T>(Predicate<T> interactionPredicate) where T : class, IInteraction
        {
            var interaction = _raisedInteractions.FirstOrDefault(i => i.GetType() == typeof(T) && interactionPredicate(i as T));
            if (interaction != null)
                Assert.Fail($"An interaction with type {typeof(T)} was raised");
        }

        private void HandleInteraction<T>(T interaction) where T : IInteraction
        {
            var type = typeof(T);
            _raisedInteractions.Add(interaction);
            if (_registrations.ContainsKey(type))
            {
                var registration = _registrations[type];
                registration(interaction);
            }
        }

        public void Raise<T>(T context) where T : IInteraction
        {
            HandleInteraction(context);

            Raised?.Invoke(this, new InteractionRequestEventArgs(context, () => { }));
        }

        public void Raise<T>(T context, Action<T> callback) where T : IInteraction
        {
            HandleInteraction(context);
            callback(context);

            Raised?.Invoke(this, new InteractionRequestEventArgs(context, () => { }));
        }

        public async Task<T> RaiseAsync<T>(T context) where T : IInteraction
        {
            var task = Task.Run(() =>
            {
                Raise(context);
                return context;
            });

            return await task;
        }

        public event EventHandler<InteractionRequestEventArgs> Raised;
    }
}
