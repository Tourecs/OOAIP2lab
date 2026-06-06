using System;

namespace OOAIP_3lab
{
    // Context that provides token and identifiers from the caller.
    public interface IAuthContext
    {
        string Token { get; }
        string GameId { get; }
        string PlayerId { get; }
    }

    // Explicit validator contract for tokens — keeps validation testable and swappable.
    public interface ITokenValidator
    {
        bool Validate(string token, string gameId, string playerId);
    }

    // Simple, safe implementation used as a default. Replace with real validation when needed.
    public sealed class SimpleTokenValidator : ITokenValidator
    {
        public bool Validate(string token, string gameId, string playerId)
        {
            return !string.IsNullOrEmpty(token);
        }
    }

    // Command that performs authorization using an injected ITokenValidator.
    public sealed class AuthCommand : ICommand
    {
        private readonly IAuthContext _context;
        private readonly ITokenValidator _validator;

        public AuthCommand(IAuthContext context, ITokenValidator validator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public void Execute()
        {
            var token = _context.Token;
            var gameId = _context.GameId;
            var playerId = _context.PlayerId;

            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("Authorization failed: token is missing.");
            }

            bool isValid = _validator.Validate(token, gameId, playerId);

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Authorization failed: invalid token or access denied.");
            }
        }
    }

    // Registration command that wires up the token validator and the auth action in IoC.
    public sealed class RegisterIoCDependencyAuth : ICommand
    {
        public void Execute()
        {
            // Register a token validator factory. Real projects should replace SimpleTokenValidator
            // with a validator that calls an auth service or checks signatures.
            Ioc.Register("Auth.TokenValidator", _ => new SimpleTokenValidator());

            // Register the auth action factory. It expects IAuthContext passed as the first argument.
            Ioc.Register("Actions.Auth", args =>
            {
                var ctx = args.Length > 0 ? args[0] as IAuthContext : null;
                if (ctx is null) throw new ArgumentException("Actions.Auth factory requires an IAuthContext as the first argument.");

                // Resolve validator lazily from IoC so consumers can override the registration if needed.
                var validator = Ioc.Resolve<ITokenValidator>("Auth.TokenValidator");
                return new AuthCommand(ctx, validator);
            });
        }
    }
}
