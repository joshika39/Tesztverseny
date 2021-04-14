# Tesztverszeny™ aka Verseny Info

Egy verseny pontozórendszere. A versenynek 14 feladata van, mindegyiknek A, B, C és D megoldása, valamint X, ha nem kerül megválaszolásra. 

*Adatok:*

Az adatokat egy file beimportálásával lehet bekérni. Ennek a file-nak az első sora a helyes megoldás, a többi pedig a versenyző azonosítója és megoldásai, a kettő szóközzel elválasztva.

```txt
BCCCDBBBBCDAAA 
AB123 BXCDBBACACADBC 
AH97 BCACDBDDBCBBCA
…
```

*Pontozás:*

A verseny feladatai nem egyenlő nehézségűek: az 1-5. feladat 3 pontot, a 6-10. feladat 4 pontot, a 11-13. feladat 5 pontot, míg a 14. feladat 6 pontot ér. 

*Helyezések:*

Holtverseny előfordulhat, tehát lehet több első, második, vagy harmadik helyezett.

*Készítette:*

- Tandi Áron
- Hegedűs Joshua
- Szobonya Dávid

*Forráskód:*

https://github.com/joshika45/tesztverseny

## Backend

### public struct Competitor

A versenyzők adatait tároló struktúra.

*Változók:*

- `public string Code` - a versenyző azonosítója.
- `public char[] Answers` - a válaszokat tartalmazó karaktertömb.
- `public int Points` - a pontszám.

### public class Importer

Osztály az adatok értelmezésére és lekérésére.

*Globális változók:*

- `private readonly List<Competitor> _competitors` - az összes versenyző adatait tároló struktúrából álló lista.
- `private char[] _correct` - a verseny helyes megoldásait tartalmazó karaktertömb.
- `public int NumCompetitors` - a versenyzők száma.

#### public void Import(string location)

Metódus a válaszokat tartalmazó file importálására, és a pontok kiiratása egy másik file-ba (`\pontok.txt`).

A versenyzők, egy [`Competitor`](#public-struct-competitor) struktúrákat tartalmazó listában vannak tárolva. Ezen kívül tárolva van még a versenyzők száma a globális `NumCompetitors` változóban, valamint a helyes megoldásokat tartalmazó karaktertömb a szintén globális, viszont privát `_correct` nevű változóban.

A versenyző pontszámát a [`CalculatePoints`](#private-int-calculatepointsireadonlylistchar-answers) függvény felhasználásával határozza meg.

*Attribútumok:*

- `string location` - a válaszokat tartalmazó file útvonala.

#### public string[] GetCompetitor(string code)

Egy függvény ami megadja egy versenyző válaszait és annak javítását.

*Attribútumok:*

- `string code` - a versenyző azonosítója.

*Visszatérési érték:*

Egy 14 stringet tartalmazó tömb. Mindegyik string egy feladatnak felel meg. Ha a megoldás helyes, akkor csak a helyes megoldást tartalmazza (pl.: "A"), különben a versenyző válaszát és a helyes megoldást (pl. "BD", "XC").

#### public string[] GetTask(int n)

Egy függvény egy feladat helyes megoldóinak számának, annak az összes versenyzőhöz képest a százalékát, valamint a helyes megoldást. 

*Attribútumok:*

- `int n` -  a feladat sorszáma.

*Visszatérési érték:*

Egy 3 stringet tartalmazó tömb. Az első a helyes megoldók száma, a második a százalék, a harmadik pedig a helyes megoldás.

#### public IEnumerable\<Competitor> GetPodium()

Egy függvény a pódiumosok meghatározására. Mivel több versenyző is lehet első, második, vagy harmadik helyezett, ezért ebben a sorrendben vannak eltárolva. A határokat az pontszámok összehasonlításával lehet megkülönböztetni. 

*Visszatérési érték:*

Egy tömb a pódiumosok adatait tartalmazó [`Competitor`](#public-struct-competitor) struktúrákkal.

#### private int CalculatePoints(IReadOnlyList\<char> answers)

Egy függvény a pontok kiszámítására. Az első öt feladat 3 pontot ér, a 10-ig 4-et, utána 5-öt, majd a legutolsó 6-ot. 

*Attribútumok:*

- `IReadOnlyList<char> answers` -  a versenyző válaszait (char) tartalmazó lista.

*Visszatérési érték:*

A versenyző pontszáma.

## Frontend

A program grafikus felhasználói felülete WPF-fel készült.
