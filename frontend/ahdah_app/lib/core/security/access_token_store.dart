abstract interface class AccessTokenStore {
  Future<String?> read();
  Future<void> write(String token);
  Future<void> clear();
}
