# MondeVirtuelProjet

ReadMe officiel : 

Ce projet est un projet de génération de rivières procéduralement. 
Dans le cadre de ce projet, nous avons générer des "routes", qui sont enfaite des riviéres de maniére procédurales.

Dans un premier lieu, nous avons générés le terrain, sous forme de cube, via des matrices et un "noise" afin d'avoir
un terrain aléatoire.

Ensuite nous avons généré des rivières de manière simple, en partant des différents point d'eau du bas et en montant petit 
à petit jusqu'au sommet des montagnes. 

Mais cette génération ne faisaient pas très naturelle donc nous avons améliora la génération des rivières en les empêchant
de se rejoindre et en ajoutant des niveaux d'élévations pour avoir un chemin naturel.

Finalement, nous avons ajoutés la création des affluents aux rivières qui se divisent de la rivière principale et
suivent leur propre chemin.