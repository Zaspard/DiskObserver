﻿using DiskObserver.Avalonia.Model.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#nullable enable

namespace DiskObserver.Avalonia.Model.Implementation {
    public sealed class QuickAccessModel : IPhysicalObject {
        public ObservableCollection<IPhysicalObject>? PhysicalObjects { get; set; } = new();

        public string Path { get => ""; set => throw new NotImplementedException(); }
        public string Name { get => ""; set => throw new NotImplementedException(); }

        public bool IsVisibleInTree => true;

        public IPhysicalObject? ParentPhysicalObject => null;

        public void Dispose() {
            if (PhysicalObjects != null && PhysicalObjects.Count > 0) {
                PhysicalObjects.Clear();
                PhysicalObjects = null;
            }
        }

        public void GetHeavyFiles(List<IFile> heavyFiles, int maxCount) {}
        public void LazyInit() {}
        public void ChangePath(string path) => throw new NotImplementedException();
        public void Delete() => throw new NotImplementedException();
    }
}
