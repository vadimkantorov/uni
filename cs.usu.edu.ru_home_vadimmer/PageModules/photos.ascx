<%@ Control Language="c#" AutoEventWireup="false" Codebehind="photos.ascx.cs" Inherits="UsuSite.PageModules.photos" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script type="text/javascript">
//<![CDATA[
		hs.graphicsDir = 'highslide/graphics/';
		hs.outlineType = 'rounded-white';
		hs.showCredits = false;
		hs.padToMinWidth = true;
		
		hs.lang = {
			loadingText :     'Загружается...',
			loadingTitle :    'Нажмите для отмены',
			focusTitle :      'Нажмите чтобы поместить на передний план',
			fullExpandTitle : 'Развернуть до оригинального размера',
			fullExpandText :  'Оригинальный размер',
			creditsText :     '',
			creditsTitle :    '',
			previousText :    'Предыдущее',
			previousTitle :   'Предыдущее (стрелка влево)',
			nextText :        'Следующее',
			nextTitle :       'Следующее (стрелка вправо)',
			moveTitle :       'Переместить',
			moveText :        'Переместить',
			closeText :       'Закрыть',
			closeTitle :      'Закрыть (esc)',
			resizeTitle :     'Изменить размер',
			playText :        'Слайдшоу',
			playTitle :       'Начать слайдшоу (пробел)',
			pauseText :       'Пауза',
			pauseTitle :      'Приостановить слайдшоу (пробел)',   
			number :          'Изображение %1 из %2',
			restoreTitle :    'Нажмите чтобы закрыть изображение, нажмите и перетащите для изменения местоположения. Для просмотра изображений используйте стрелки.'
		};
//]]>
</script>
<div class="post">
	<h1 class="title">Фото из Эрфурта перед рождеством</h1>
	<asp:DataList runat="server" ID="dlPhotos" RepeatColumns="4">
		<ItemTemplate>
			<a href='images/full/<%#DataBinder.Eval(Container.DataItem, "FullImageName")%>' class="highslide" onclick="return hs.expand(this)">
					<img src='images/tmb/<%#DataBinder.Eval(Container.DataItem, "FullImageName")%>' alt="Highslide JS"
						title="Увеличить!" height="120" width="107" />
				</a>
		</ItemTemplate>
	</asp:DataList>
</div>
