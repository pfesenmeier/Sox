namespace Sox;

public class RuntimeError : Exception {
    readonly Token token;

    RuntimeError(Token token, String message): base(message) {
      this.token = token;
    } 
}
