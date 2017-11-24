using System;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DTO;
using TellMe.Core.Contracts.UI;
using TellMe.Core.Contracts.UI.Views;
using TellMe.iOS.Core;

namespace TellMe.iOS.Controllers
{
	public partial class EventViewController : StoriesTableViewController, IEventView
	{
		private new IEventViewBusinessLogic BusinessLogic
		{
			get => (IEventViewBusinessLogic) base.BusinessLogic;
			set => base.BusinessLogic = value;
		}

		public EventViewController(IntPtr handle) : base(handle)
		{
		}

		public event EventDeletedHandler EventDeleted;
		public EventDTO Event { get; set; }
		public int EventId { get; set; }

        
		public override void ViewDidLoad()
		{
			if (this.BusinessLogic == null)
				this.BusinessLogic = IoC.GetInstance<IEventViewBusinessLogic>();

			base.ViewDidLoad();
		}

		public override void ViewWillAppear(bool animated)
		{
			this.NavigationController.SetToolbarHidden(true, false);
		}

		public void DisplayEvent(EventDTO eventDTO)
		{
			InvokeOnMainThread(() =>
			{
				//NavItem.Title = tribe.Name;
				/*this.EventName.Text = eventDTO.Title;
				this.EventDescription.Text = eventDTO.Description;*/
			});
		}

		void IEventView.EventDeleted(EventDTO eventDTO)
		{
			EventDeleted?.Invoke(eventDTO);
		}
	}
}