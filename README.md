# Шаблон проектирования MVVM

Одной из типовых проблем при разработке Desktop-приложений (а также мобильных приложений) является высокая сцепленность кода (_coupling_). На практике высокая сцепленность приводит к засорению пространства идентификаторов органов управления в пользовательском интерфейсе, высокой сложности сопровождения кода, усложнению повторного использования кода. Проявлением «сильной сцепленности» является ситуация, когда изменение в одном компоненте вынуждает вносить изменения и в других компонентах.

Шаблон проектирования **MVVM** предназначен для устранения подобных проблем. Шаблон предполагает разделение ответственности (_Separation of Concerns_) между тремя основными участниками:

1. Модель (Model) – это описание используемых сущностей (Entities) как объектов с определённым набором полей. Model не содержит никакой бизнес-логики – это просто контейнеры связанных между собой полей. Примером модели может быть объект, описывающий принятую пачку наличных
2. Отображение (View) – класс, отображающий данные в определённом виде, например, таблицу принятых номиналов, или список подключенных счётчиков
3. Связующий слой (ViewModel) – набор классов, которые отвечают за выполнение некоторых бизнес-операций, включая доступ к базе данных и взаимодействие с аппаратными узлами

Связующий слой (ViewModel) так же можно разделить на несколько слоёв:

1. Контекст данных, к которому могут обращаться различные View и другие классы из ViewModel
2. Обработчики команд, которые оператор может подать через View
3. Helper-ы, классы обеспечивающие выполнение запросов к базе данных, а также доступа к различным устройствам

Предположим, что мы разрабатываем приложение-аггрегатор, которое умеет взаимодействовать с разными платёжными шлюзами, выполняя перевод денежных средств конкретному получателю. «Слабая сцепленность» обеспечивается тем, что при добавлении нового платёжного шлюза с уникальным API добавляется новый Helper-класс и обработчик команды, но сама команда (выполнение платежа) остаётся неизменной. View и Model также не изменяются. Т.е. изменения затрагивают только относительно небольшую часть программного кода и их стоимость будет минимальной.

## Этапность выполнения работ при использовании MVVM

Рекомендуется сначала определить сущности, входящие в модель. Такими сущностями могут быть: описание валюты (идентификатор, цифровой код валюты, буквенный код валюты), количество купюр определённого номинала, принятых в рамках определённой транзакции, транзакция внесения наличных (идентификатор транзакции, к которому привязывается информация о количестве внесённых купюр, дата начала операции, результат выполнения операции, и т.д.).

Следующий шаг – заглушки для View и ViewModel. Под заглушками подразумевается минимально необходимый код, который позволяет убедиться, что коммуникационные каналы между частями приложения настроены корректно и работают. Пример:

* Разработать поле для ввода строки фильтрации данных
* Разработать часть ViewModel в которую будет сохранено введённое значение
* Убедиться под отладчиком, что введённое значение сохраняется в переменную
* Разработать класс, выполняющий команду (Search) во ViewModel
* Связать View с классом для кнопки «Search» во ViewModel
* Добавить во ViewModel **ObservableCollection** для конкретной Model, в который будет помещён результат выполнения команды «Search»
* Добавить во View код, который отображает список из ObservableCollection
* Убедиться, что вся цепочка работает корректно

Далее следует доработать View с целью добиться желаемого отображения данных.

На последнем этапе – добавляются Helper-ы, которые выполняют запросы к сетевым ресурсам, устройствам и к базе данных.

## Пример приложения BanknotesMVVM

Начальное приложение следует сгенерировать, используя генератор кода по шаблонам из состава Microsoft Visual Studio. Параметры шаблона: XAML, Windows, Desktop. В качестве Runtime рекомендуется использовать .NET 5.0 – последнюю из доступных версий .NET.

При разработке View, рекомендуется группировать связанные сущности (_cohesion_) в UserControl. Кажется разумным создать отдельную папку «Views», в которой размещать разработанные UserControl-ы. Создать UserControl рекомендуется посредством генератора компонентов приложения из состава Visual Studio, см. [статью](https://www.wpf-tutorial.com/usercontrols-and-customcontrols/creating-using-a-usercontrol/).

В соответствии с шаблоном проектирования MVVM, следует избегать добавления какого-либо программного кода в CS-файлы, описывающие View. View - это только верстка, тогда, как программный код (CS) - это модель данных и ViewModel.

## View

Ниже приведён пример XAML-верстки:

```xaml
<UserControl x:Class="BanknotesMVVM.Views.BanknotesView" ...
             d:DesignHeight="450" d:DesignWidth="800">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*"/>
            <ColumnDefinition Width="Auto"/>
        </Grid.ColumnDefinitions>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>
        <TextBox Grid.Column="0" Grid.Row="0" Text="0" Margin="0,0,0,10" />
        <Button Grid.Column="1" Grid.Row="0" Content="Start Cash In" />
        <ListBox Grid.Column="0" Grid.Row="1" Grid.ColumnSpan="2" />
    </Grid>
</UserControl>
```

Этот UserControl может быть включен в соответствующее окно. Например, в главное окно приложения:

```xaml
<Window x:Class="BanknotesMVVM.MainWindow" ...
		xmlns:uc="clr-namespace:BanknotesMVVM.Views"
		Title="MainWindow" Height="450" Width="800">
	<Grid>
		<uc:BanknotesView />
	</Grid>
</Window>
```

Следует заметить, что при разработке View следует применять композицию - собирать View из отдельных компонентов, элементы которых обладают логической связанностью.

## ViewModel

Рекомендуется создать в проекте папку «ViewModel» и внутри неё папку «Helpers». В первой папке будет находится реализация классов, относящихся к уровню ViewModel, а в дочерней папке «Helpers» - файлы, в которых будет определён программный код, осуществляющий доступ к базам данных, серверам (API), а также к другим источникам данных.

Для начала работы рекомендуется создать в папке «ViewModel» файл «CashInVM.cs» - класс, относящийся к уровню ViewModel, в котором будет хранится значение переменной EnteredValue. Пример реализации приведён ниже:

```csharp
using System.ComponentModel;
namespace BanknotesMVVM.ViewModel {
	class CashInVM : INotifyPropertyChanged	// Суффикс VM означает ViewModel
	{
		private string enteredValue;
		public string EnteredValue {
			get { return enteredValue; }
			set {	// При изменении свойства, значение будет отправлено подписчикам
				enteredValue = value;
				OnPropertyChanged("EnteredValue");
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		// Отправка изменённого значения подписчикам
		private void OnPropertyChanged(string propertyName) {
			PropertyChanged?.Invoke(this,
				new PropertyChangedEventArgs(propertyName));
		}
	}
}
```

Для того, чтобы связать строку ввода, определённую в XAML на уровне View, следует добавить пространство имён ViewModel в XAML:

```xaml
<UserControl x:Class="BanknotesMVVM.Views.BanknotesView" ...
             xmlns:vm="clr-namespace:BanknotesMVVM.ViewModel" ...>
```

Следующий шаг – мы определяем ресурс, которым является разработанный ранее класс «CashInVM.cs» и называем его «vm»:

```xaml
<UserControl.Resources>
	<vm:CashInVM x:Key="vm"/>
</UserControl.Resources>
```

Далее необходимо указать контекст данных (DataContext) для конкретного подмножества XAML-элементов. Под контекстом данных подразумевается конкретный экземпляр класса, в котором находятся свойства, используемые для связывания (binding). Например, это можно сделать для Grid-а:

```xaml
<Grid DataContext="{StaticResource vm}">
```

После этого, мы можем использовать свойства класса CashInVM для операций связывания, например:

```xaml
<Grid DataContext="{StaticResource vm}">
	<TextBox Text="{Binding EnteredValue, Mode=TwoWay}"/>
```

Цель всей приведённой выше конструкции состоит в том, что определено некоторое состояние (переменная), которое может быть использовано как входное значение для Helper-функций. В этоу перменную (состояние) можно будет поместить результат выполнения Helper-функций. Описание пользовательского интерфейса (Modal) является декларативным и не содержит программного кода на C#. Любые изменения свойства EnteredValue на уровне ViewModel (бизнес-логика) будут автоматические отображаться в органах управления на уровне Modal и не потребуются какие-либо дополнительные синхронизационные действия.

## Добавление обработчиков команд

Команды – это классы ViewModel, которые связаны с командными кнопками в XAML и реагируют на нажатие кнопок используя свойства ViewModel (см. EnteredValue) для запуска Helper-функций.

Для реализации команд рекомендуется создать отдельную папку внутри «ViewModel» под названием «Commands».

Пример реализации команды:

```csharp
using System;
using System.Windows.Input;
namespace BanknotesMVVM.ViewModel.Commands {
	class FillCommand : ICommand	{
		public CashInVM VM { get; set; }
		public event EventHandler CanExecuteChanged;

		// Сохраняем ссылку на ViewModel
		public FillCommand(CashInVM vm) {
			VM = vm;
		}
		// Определяем условия, при которых команда может быть выполнена
		public bool CanExecute(object parameter)  {
			return true;
		}
		// Делегируем выполнение команды во ViewModel
		public void Execute(object parameter) {
			VM.GenerateData();
		}
	}
}
```

При создании экземпляра класса FillCommand ему нужно передать ссылку на главный экземпляр ViewModel – эту ссылку следует сохранить для обеспечения возможности проверки условий выполнения команды (CanExecute), а также непосредственно для выполнения команды (Execute).

```csharp
class CashInVM : INotifyPropertyChanged {
	// Конструктор
	public CashInVM() {
		// Создаём экземпляр команды, чтобы его можно было
		// безопасно использовать в XAML
		FillCommand = new FillCommand(this);
	}
	// Команды
	public FillCommand FillCommand { get; set; }
	...
	// Определяем действия, которые можно использовать в командах XAML
	public void GenerateData()
	{
	}
```
  
Заметим, что экземпляр команды (instance) создаётся в конструкторе ViewModal. Этот подход позволяет использовать фабрику команд, основываясь, например, на конфигурационных файлах. Предположим, что в разрабатываемом приложении должна быть реализована функция экспорта данных в Excel-таблицу, но для разных заказчиков формат таблицы является разным. В зависимости от настроек системы, в конструкторе будет создаваться экземпляр соответствующей команды экспорта в Excel.

После того, как экземпляр команды создан в ViewModel, можно использовать её в XAML-коде View:

```xaml
<Button Grid.Column="1" Grid.Row="0" Content="Start Cash In"
	Command="{Binding FillCommand}" />
```

Подтвердить работоспособность кода можно добавив следующий код в метод ViewModel GenerateData():

```csharp
public void GenerateData() {
	Console.WriteLine($"Entered Value = {EnteredValue}");
	EnteredValue = "";
}
```

Дополнительно можно добавить код, который будет блокировать возможность выполнения команды по некоторым условиям, например, команду можно будет выполнять только в том, случае, если в строке редактирования (фактическое значение EnteredValue) будут введены какие-то данные. 

Для этого сначала необходимо добавить в команду код подписки на событие CanExecuteChanged:

```csharp
class FillCommand : ICommand {
	public event EventHandler CanExecuteChanged {
		// Необходимо выполнить подписку на событие при добавлении
		// и удалении элемента. Если этого не сделать, то CanExecute()
		// не будет вызываться
		add { CommandManager.RequerySuggested += value; }
		remove { CommandManager.RequerySuggested -= value; }
	}
```

Так же нужно добавить осмысленное условие для принятия решения о том, разрешать команду, или нет. Пример такого условия – не разрешать команду, если данные не были введены:

```csharp
public bool CanExecute(object parameter) {
	if (parameter is String) {
		// Разрешаем выполнить команду только в том случае, 
		// если какие-то данные были введены
		return ((parameter as String).Length != 0);
	}
	return false;
}
```

Чтобы не возникло исключение, проверяемые значения следует передавать через параметр вызова в CanExecute(). Для этого следует соответствующим образом настроить XAML-команду:

```xaml
<Button Grid.Column="1" Grid.Row="0" Content="Start Cash In"
	Command="{Binding FillCommand}" CommandParameter="{Binding EnteredValue}" />
```

Ещё одно важное замечание – в случае использования CanExecute(), рекомендуется обновлять значение свойства EnteredValue по событию PropertyChanged:

```xaml
<TextBox Grid.Column="0" Grid.Row="0" Margin="0,0,0,10"
	 Text="{Binding EnteredValue, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
```

Как результат, кнопка «Start Cash In» будет заблокирована если в строке редактирования текст не введён и разрешена, если какой-либо текст введён.

Целью приведённых выше действий является обеспечение декларативного описания пользовательского интерфейса (View) и модели (Model), т.е. описания элементов из которых они состоят (наборы полей), а не алгоритмов, которые позволяют достичь поставленных результатов. В свою очередь, ModelView разделяется на несколько слабо связанных классов:

*	Интерфейсы команд, которые проверяют условия возможности выполнения команды
*	Helper-функции, которые обращаются к внешним серверам, устройствам и базам данных
*	Связующий слой, реализующий бизнес-логику

Разделение приложения на слабое связанные компоненты позволяет упростить решение задачи в целом, повысить степень повторного использования компонентов и легче разделять задачу по разным исполнителям.

## The ObservableCollection

Класс ObservableCollection<T> является списком, который учитывает внесение изменений. Внесение изменений связано с binding-ом. По интерфейсу использования этот класс очень похож на List<T>.
  
Этот класс имеет смысл использовать в ситуациях, когда следует обрабатывать добавление, или удаление элементов списка.

Предположим, что при нажатии на кнопку «Start Cash In» мы отправили команду счётчику на пересчёт наличных и через некоторое время счётчик сообщил нам результат. Реализацию протокола взаимодействия со счётчиком следовало бы реализовать в отдельном Helper-классе в рамках ViewModal. Функцию выполнения операции CashIn следовало бы сделать асинхронной (чтобы обеспечить отзывчивость системы и улучшить утилизацию вычислительных ресурсов системы). Предположим, что результатом работы является список принятых наличных в разбивке по номиналам.

Предположим, что ранее мы уже разработали модель, в которой определён класс Notes с несколькими атрибутами: номинал купюры, количество купюр в пачке и имя валюты.
Для хранения результатов пересчёта следует использовать ObservableCollection<Notes>, т.е. фактически, список экземпляров класса Notes, размещённые в контейнере, похожем на List, но поддерживающим уведомление подписчиков об изменении содержимого контейнера.
  
В ViewModel следует добавить контейнер соответствующего типа и проинициализировать его в конструкторе:

```csharp
class CashInVM : INotifyPropertyChanged {
	public CashInVM() { ...
		Batch = new ObservableCollection<Notes>();
	}
	// Результаты выполнения команд пользователя
	public ObservableCollection<Notes> Batch { get; set; }
```

Критически важным является сохранение экземпляра коллекции. Мы не можем присвоить контейнеру Batch новое экземпляр без потери связи с подписчиками! Соответственно, изменение содержимого контейнера после выполнения очередной операции должно выполняться по следующей схеме:

```csharp
public void GenerateData() {
	// Сбрасываем предыдущий результат выполнения операции
	Batch.Clear();
	// Параметры вызова: номинал, количество, код валюты...
	// Вместо имени валюты выводим текст из строки ввода
	Batch.Add(new Models.Notes(50, 4, 810, EnteredValue));
	Batch.Add(new Models.Notes(100, 13, 810, EnteredValue));
	Batch.Add(new Models.Notes(200, 1, 810, EnteredValue));
	// Сбрасываем содержимое строки редактирования
	EnteredValue = "";
}
```

Следующим действием следует создать во View список для отображения результатов и описать шаблон оформления отдельной записи:

```xaml
<ListView Grid.Column="0" Grid.Row="1" Grid.ColumnSpan="2"
		  ItemsSource="{Binding Batch}" SelectedValue="{Binding SelectedNotes}">
	<ListView.ItemTemplate>
		<DataTemplate>
			<Grid>
				<StackPanel Orientation="Horizontal">
					<TextBlock Text="{Binding TotalAmount}" Margin="0,0,5,0" />
					<TextBlock Text="{Binding Currency.CurrencyName}"/>
				</StackPanel>
			</Grid>
		</DataTemplate>
	</ListView.ItemTemplate>
</ListView>
```

В атрибуте ItemsSource мы указываем имя контейнера Batch из ViewModel. Поскольку ListView находится внутри контекста данных CashInVM, то контейнер Batch будет доступен. См.:

```csharp
<Grid DataContext="{StaticResource vm}">
```
  
В определении шаблона описания записи (см. DataTemplate) мы выполняем связывание с полями экземпляра класса Notes, которые хранятся в контейнере Batch.

В приведённом выше примере используется ещё один атрибут – SelectedValue, который позволяет сохранять выбранный пользователем элемент в списке. Для того, чтобы код работал, нам потребуется добавить во ViewModel дополнительное свойство:

```csharp
class CashInVM : INotifyPropertyChanged { …
	private Notes selectedNotes;
	public Notes SelectedNotes {
		get { return selectedNotes; }
		set {
			selectedNotes = value;
			OnPropertyChanged("SelectedNotes");
		}
	}
```

## Использование Entity Framework
	
В отличие от ASP.NET Core в WPF-шаблонах приложений поддержка Dependancy Injection не встроена изначально. В ASP.NET ядро контролирует создание страницы (PageModel), выполняя Dependency Injection через конструктор применяя **Reflection**.
	
В приложении на WPF необходимо реализовать свой собственный "огород", см. IHost и GetRequiredService().
	
# Резюмируя

Применение шаблона проектирования MVVM позволяет разделить код приложения на множество слабосвязанных компонентов с чётко определёнными зонами ответственности:

*	View отвечает за описание пользовательского интерфейса и не содержит никакой бизнес-логики
*	Model – определяет структуру получаемых из разных источников данных
*	Helpers – вспомогательные классы, которые реализуют взаимодействие с конкретными источниками данных (в том числе, с аппаратными устройствами)
*	ViewModel – связующий уровень, который хранит состояние приложения и выполняет некоторую бизнес-логику, например, в ответ на некоторое действие пользователя выполняет запрос во внешнюю систему и передаёт полученные данные во View

Важно заметить, что в приложении может быть не ограниченное количество View, Models, Helpers и ViewModels.

# Ссылки для дополнительного изучения

* [Matanit: Команды и взаимодействие с пользователем в MVVM. 2021](https://metanit.com/sharp/xamarin/4.3.php)
* [Habr: MVVM: полное понимание (+WPF). 2017](https://habr.com/ru/post/338518/)
	
В 2016 году на Metanit вышла статья [Команды в MVVM](https://metanit.com/sharp/wpf/22.3.php). Предлагаемое в статье решение предлагает использовать специализированный класс **RelayCommand** (командный переключатель), который реализует поведение методов CanExecute() и Execute(), а также обработчика событий CanExecuteChanged(). Реализация класса следующая:
	
``` csharp
namespace MVVM
{
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;
 
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
 
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
 
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }
 
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
```

Этот класс используется во ViewModel и позволяет обеспечить доступ из реализации функтора, отрабатывающего команду, доступ к атрибутам ViewModal. Например:
	
``` csharp
public class ApplicationViewModel : INotifyPropertyChanged
{
	private Notes selectedNotes;
	public ObservableCollection<Notes> Notes { get; set; }

	private RelayCommand addCommand;
	public RelayCommand AddCommand
	{
		get
		{
			return addCommand ??
			  (addCommand = new RelayCommand(obj =>
			  {
				  Notes Notes = new Notes();
				  Notes.Insert(0, Notes);
			  }));
		}
	}
```

В приведённом выше примере, getter внутреннего класса AddCommand имеет доступ к коллекции Notes.
	
В простейшем случае, вариант от Metanit-а и реализованный в данном репозитации, приводят к одинаковому результату. На мой взгляд, вариант с Metanit-а хуже по двум причинам: он приводит к захламлению кода ViewModal по мере добавления новых команд, а также он менее гибкий. Низкая гибкость состоит в том, что в варианте от Matinit-а реализовать уникальное поведение CanExecute(), Execute() и CanExecuteChanged() нельзя - оно уже жёстко определено в реализации RelayCommand. Например, если потребуется сделать команду доступной по условию, то в варианте от Metanit изменить поведение поведение в CanExecute() нельзя, т.к. оно общее для всех команд. Потребуется передавать свойство enabled/disabled через дополнительный атрибут XAML-элемента (см. CommandParameter):

```xaml
<Button Grid.Column="1" Grid.Row="0" Content="Start Cash In" 
	Command="{Binding FillCommand}" CommandParameter="{Binding EnteredValue}" />
```
	
В C# коде значение атрибута передаётся через parameter: 
	
```csharp
public bool CanExecute(object parameter)
{
        return this.canExecute == null || this.canExecute(parameter);
}
```

Фактически, во ViewModel должен быть определен атрибут (один), который будет принимать логическое значение (true/false). В большинстве случаев этого хватит для реализации требований заказчиков, но когда не хватит, потребуется создавать вычисляемое свойство.

Резюмируя – в простейшем случае, решение от Метанита будет работать, но также будет провоцировать создание костылей, которые нужно будет делать очень аккуратно. По сути, разработчики примера с Метанита заявили, что создание отдельных методов CanExecute(), Execute() и CanExecuteChanged() не нужно, можно обойтись только одним функтором. Это смелое заявление, т.е. репутация Metanit не такого же порядка, как репутация Microsoft.
	
И ещё одно замечание – чтобы полноценно использовать решение Metanit нужно весьма хорошо знать WPF, в частности, понимать, зачем нужен атрибут CommandParameter.
