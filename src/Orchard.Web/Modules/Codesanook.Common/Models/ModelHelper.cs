using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeSanook.Common.Models
{
    public static class ModelHelper
    {
        public static void UpdateModel<TModel>(
            IList<TModel> exisitingModels,
            IList<TModel> newModels,
            Action<IList<TModel>> addAction,
            Action<IList<TModel>> removeAction,
            Action<IList<TModel>> updateAction) where TModel : Entity<int>
        {

            var toAddModels = newModels.Except(exisitingModels).ToList();
            var toRemoveModels = exisitingModels.Except(newModels).ToList();
            var toUpdateModels = exisitingModels.Intersect(newModels).ToList();

            HandleAddAction(toAddModels, addAction);
            HandleRemoveAction(toRemoveModels, removeAction);
            HandleUpdateAction(toUpdateModels, updateAction);
        }

        private static void HandleAddAction<TModel>(IList<TModel> toAddModels, Action<IList<TModel>> addAction) where TModel : Entity<int>
        {
            if (ValidateInput(toAddModels, addAction))
            {
                addAction(toAddModels);
            }
        }

        private static void HandleRemoveAction<TModel>(IList<TModel> toRemoveModels, Action<IList<TModel>> removeAction) where TModel : Entity<int>
        {
            if (ValidateInput(toRemoveModels, removeAction))
            {
                removeAction(toRemoveModels);
            }
        }

        private static void HandleUpdateAction<TModel>(IList<TModel> toUpdateModels, Action<IList<TModel>> updateAction) where TModel : Entity<int>
        {
            if (ValidateInput(toUpdateModels, updateAction))
            {
                updateAction(toUpdateModels);
            }
        }

        private static bool ValidateInput<TModel>(IList<TModel> toAddModels, Action<IList<TModel>> addAction)
        {
            return toAddModels != null && toAddModels.Count > 0 && addAction != null;
        }
    }
}