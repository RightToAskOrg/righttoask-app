#!/bin/sh

# Decrypt the file
mkdir $HOME/secrets
# --batch to prevent interactive command
# --yes to assume "yes" for questions
gpg --quiet --batch --yes --decrypt --passphrase="$AES_KEY_PASSPHRASE_FOR_MPS_FILE" \
	--output $HOME/RightToAskClient/Resources/MPs.json  $HOME/RightToAskClient/Resources/MPs.json.encrypted.gpg 


