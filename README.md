# RSS Reader

Desktop-приложение, написанное на C#7.3 и WPF 4.5, для просмотра RSS-лент.

![](https://sun9-19.userapi.com/c205228/v205228452/6fbfa/Aal2GAxiggI.jpg)

### Требования к работе программы:
  - .NET Framework 4.5

### Как пользоваться?
* Запустите программу
* Чтобы добавить свои источники:
    * нажмите на кнопку **Настройки** во время работы программы.
* **ИЛИ**
    * закройте программу, откройте файл **data.xml**, отредактируйте его, запустите программу.

![](https://sun9-32.userapi.com/c205228/v205228452/6fc59/tL6w41ThCzE.jpg)
При первом запуске произойдет (или при неправильно отконфигурированном **data.xml**) создание файла с **data.xml** в той же директории, что и программа, с единственным RSS-источником (https://habr.com/ru/rss/interesting/).

### Пример *data.xml*
```xml
<?xml version="1.0" encoding="utf-8"?>
<items>
    <parameter>
        <source>https://habr.com/ru/rss/interesting/</source>
        <updateInterval>60</updateInterval>
    </parameter>
    <parameter>
        <source>https://habr.com/ru/rss/all/</source>
        <updateInterval>60</updateInterval>
    </parameter>
    <parameter>
        <source>https://lenta.ru/rss/news</source>
        <updateInterval>30</updateInterval>
    </parameter>
</items>
```
+   __source__ :
    + Ссылка на RSS-исчточник.
    + Пример: _https://habr.com/ru/rss/interesting/_
+   __updateInterval__ :
    + Интервал обновления источника в секундах.
    + Пример: _120_ (обновление этого источника раз в 2 минуты (120 секунд)
* Количество объектов __parameter__ может быть неограниченным.

