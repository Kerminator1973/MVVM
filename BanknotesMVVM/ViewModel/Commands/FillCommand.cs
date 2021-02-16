using System;
using System.Windows.Input;

namespace BanknotesMVVM.ViewModel.Commands
{
    class FillCommand : ICommand
	{
		public CashInVM VM { get; set; }
        public event EventHandler CanExecuteChanged
        {
			// Необходимо выполнить подписку на событие при добавлении
			// и удалении элемента. Если этого не сделать, то CanExecute()
			// не будет вызываться
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		// Сохраняем ссылку на ViewModel
		public FillCommand(CashInVM vm)
		{
			VM = vm;
		}

		// Определяем условия, при которых команда может быть выполнена
		public bool CanExecute(object parameter)
		{
			if (parameter is String)
            {
				// Разрешаем выполнить команду только в том случае, 
				// если какие-то данные были введены
				return ((parameter as String).Length != 0);
			}

			return false;
		}

		// Делегируем выполнение команды во ViewModel
		public void Execute(object parameter)
		{
			VM.GenerateData();
		}
	}
}
