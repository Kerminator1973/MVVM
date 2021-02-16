using BanknotesMVVM.Models;
using BanknotesMVVM.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanknotesMVVM.ViewModel
{
	// Суффикс VM означает ViewModel
	class CashInVM : INotifyPropertyChanged
	{
		// Конструктор
		public CashInVM()
		{
			// Создаём экземпляр команды, чтобы его можно было
			// безопасно использовать в XAML
			FillCommand = new FillCommand(this);

			// Создаём контейнер для результатов выполнения команд.
			// Контейнер должен быть проинициализирован в конструкторе и
			// не должен пересоздаваться, т.к. при пересоздании будут
			// потеряны установленные с подписчиками связи
			Batch = new ObservableCollection<Notes>();
			Batch.Add(new Models.Notes(10, 20, 810, "RUB"));
		}

		// Команды
		public FillCommand FillCommand { get; set; }

		// Свойства...

		// Введённое пользователем значение в строке редактирования
		private string enteredValue;
		public string EnteredValue
		{
			get { return enteredValue; }
			set
			{
				enteredValue = value;
				OnPropertyChanged("EnteredValue");
			}
		}

		// Выбранный пользователем элемент в списке
		private Notes selectedNotes;
		public Notes SelectedNotes
		{
			get { return selectedNotes; }
			set
			{
				selectedNotes = value;
				OnPropertyChanged("SelectedNotes");
				
				// Здесь можно добавить какие-то дополнительные действия,
				// в том числе, с пользовательским интерфейсом
			}
		}

		// Механизмы обеспечения связывания компонентов системы (binding)
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this,
				new PropertyChangedEventArgs(propertyName));
		}

		// Определяем действия, которые можно использовать в командах XAML
		public void GenerateData()
		{
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

		// Результаты выполнения команд пользователя
		public ObservableCollection<Notes> Batch { get; set; }
	}
}
