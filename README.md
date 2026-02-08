## Архитектура
**TestModels** - Здесь хранятся модели. Каждая категория устройства использует свой тип, ведь у каждого могут быть свои свойства, да и так правильнее, как по мне  

**TestViewModels** - Проект для управления бизнес-логикой. Вся навигация между страницами и диалогами тут.  
Благодаря этому, мы можем слёгкостью мигрировать на любой другой фреймворк, достаточно лишь будет описать UI, к примеру на [Uno](https://github.com/unoplatform/uno) или [AvaloniaUI](https://github.com/AvaloniaUI/Avalonia).  
Многие свойства остались от копирования с моего личного проекта по [AvaloniaUI](https://github.com/AvaloniaUI/Avalonia).  
JSON сериализуется не по закрытию, а по изменениям в коллекции. По закрытию - излишне.  
Не было использовано нативное диалоговое окно WPF, которое показывается через **ShowDialog** на новом окне, вместо этого, было шаблонизировано диалоги внутри XAML с привязкой к ViewModels  

**TestTasks** - WPF проект для UI ссылающийся на 2 проекта выше.

## Что можно было бы добавить
**Dependency Injection** - для более серьёзного подхода, необходим. Особенно в кросс-платформе.  
Разделить ресурсы на раздельные файлы. Например: **ConverterResources.xaml** где будут расположено все конвертеры и т.д.  
Увести ресурсы страницы в общие ресурсы, чтобы увеличить область видимости.  
Добавить другие фреймворки чтобы был выбор и расширить область запуска на разных ОС, включая мобильные устройства.

## Требуется:
[.NET 10 SDK](https://dotnet.microsoft.com/ru-ru/download/dotnet/10.0) для компиляции  
[Visual Studio](https://visualstudio.microsoft.com/ru/) желательно 2026  
[.NET 10 Desktop Runtime](https://dotnet.microsoft.com/ru-ru/download/dotnet/10.0) - для запуска. Включён в состав [.NET 10 SDK](https://dotnet.microsoft.com/ru-ru/download/dotnet/10.0)  
Запускать файл [TestTasks.exe](https://github.com/FrozDark/TestTasks/blob/master/App/TestTasks.exe) в папке [App](https://github.com/FrozDark/TestTasks/tree/master/App)
