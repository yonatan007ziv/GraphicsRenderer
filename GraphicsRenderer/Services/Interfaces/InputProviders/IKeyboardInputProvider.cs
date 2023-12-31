﻿using GraphicsRenderer.Components.Shared.Input;

namespace GraphicsRenderer.Services.Interfaces.InputProviders;

public interface IKeyboardInputProvider
{
	bool IsKeyDown(KeyboardButton keyboardButton);
}