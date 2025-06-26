enum Currency {
  None,
  Coins = "Coins",
  Gems = "Gems",
  Trophies = "Trophies",
  Hard = "Hard",
};

let DefaultWallet = {
  [Currency.Coins]: 1000,
  [Currency.Gems]: 100,
  [Currency.Trophies]: 0,
};

function storeUserWallet(nk: nkruntime.Nakama, user_id: string, changeset: { Coins: number; Gems: number; Trophies: number; }, logger: nkruntime.Logger) {
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


function getCurrencyInWallet(nk: nkruntime.Nakama, userId: string, currencyKeyName: Currency): number {

  var amountToReturn = 0;

  try {
    let results = nk.walletLedgerList(userId);

    switch (currencyKeyName) {
      case Currency.Coins:
        amountToReturn = results.items[0].changeset[Currency.Coins]
        break;
      case Currency.Gems:
        amountToReturn = results.items[0].changeset[Currency.Gems]
        break;
      case Currency.Trophies:
        amountToReturn = results.items[0].changeset[Currency.Trophies]
        break;
    }

    return amountToReturn;

  } catch (error) {
    // Handle error
  }

  return amountToReturn;
}