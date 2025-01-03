enum Currency {
  Coins = "coins",
  Gems = "gems",
  Trophies = "trophies",
  Hard = "hard",
};

let DefaultWallet = {
  [Currency.Coins]: 1000,
  [Currency.Gems]: 100,
  [Currency.Trophies]: 0,
};

function storeUserWallet(nk: nkruntime.Nakama, user_id: string, changeset: { coins: number; gems: number; trophies: number; }, logger: nkruntime.Logger) {
  try {
      nk.walletUpdate(user_id, changeset);
  } catch (error) {
      logger.error('Error storing wallet of player : %s', user_id);
  }
}

function updateWalletWithCurrency(nk: nkruntime.Nakama, userId: string, currencyKeyName: Currency, amount: number): nkruntime.WalletUpdateResult {
  const changeset = {
    [currencyKeyName]: amount,
  }
  let result = nk.walletUpdate(userId, changeset);

  return result;
}