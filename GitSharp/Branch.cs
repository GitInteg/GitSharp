﻿/*
 * Copyright (C) 2009, Henon <meinrad.recheis@gmail.com>
 *
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or
 * without modification, are permitted provided that the following
 * conditions are met:
 *
 * - Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 *
 * - Redistributions in binary form must reproduce the above
 *   copyright notice, this list of conditions and the following
 *   disclaimer in the documentation and/or other materials provided
 *   with the distribution.
 *
 * - Neither the name of the Git Development Community nor the
 *   names of its contributors may be used to endorse or promote
 *   products derived from this software without specific prior
 *   written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */


using System;

using CoreRef = GitSharp.Core.Ref;
using CoreCommit = GitSharp.Core.Commit;
using CoreTree = GitSharp.Core.Tree;
using CoreRepository = GitSharp.Core.Repository;

namespace GitSharp
{
    public class Branch : Ref
    {
        private const ResetBehavior DEFAULT_RESET_BEHAVIOR = ResetBehavior.Mixed;

        public Branch(Ref @ref)
            : base(@ref._repo, @ref.Name)
        {
        }

        public Branch(Repository repo, string name)
            : base(repo, name)
        {
        }

        internal Branch(Repository repo, CoreRef @ref)
            : this(repo, @ref.Name)
        {
        }

        /// <summary>
        /// Returns the latest commit on this branch.
        /// </summary>
        public Commit CurrentCommit
        {
            get
            {
                return Target as Commit;
            }
        }

        /// <summary>
        /// True if this ref points to a remote branch.
        /// </summary>
        public bool IsRemote
        {
            get;
            internal set;
        }

        public void Merge(Branch other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete this branch
        /// </summary>
        public void Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Switch to this branch and check it out into the working directory.
        /// </summary>
        public void Checkout()
        {
            Reset(ResetBehavior.Hard);
        }

        public void Reset()
        {
            Commit commit = GetSafeCurrentCommit();
            Reset(commit);
        }

        public void Reset(string commitHash)
        {
            Commit commit = ResolveCommit(commitHash);
            Reset(commit);
        }

        public void Reset(Commit commit)
        {
            Reset(commit, DEFAULT_RESET_BEHAVIOR);
        }

        public void Reset(ResetBehavior resetBehavior)
        {
            Commit commit = GetSafeCurrentCommit();
            Reset(commit, resetBehavior);
        }

        public void Reset(string commitHash, ResetBehavior resetBehavior)
        {
            Commit commit = ResolveCommit(commitHash);
            Reset(commit, resetBehavior);
        }

        public void Reset(Commit commit, ResetBehavior resetBehavior)
        {
            if (commit == null)
            {
                throw new ArgumentNullException("commit");
            }

            switch (resetBehavior)
            {
                case ResetBehavior.Hard:
                    ResetHard(commit);
                    break;

                case ResetBehavior.Soft:
                    ResetSoft(commit);
                    break;

                case ResetBehavior.Mixed:
                case ResetBehavior.Merge:
                    throw new NotImplementedException();

                default:
                    throw new NotSupportedException(string.Format("{0} is not supported.", resetBehavior));
            }
        }

        private Commit ResolveCommit(string commitHash)
        {
            var commit = new Commit(_repo, commitHash);

            if (!commit.IsCommit)
            {
                throw new ArgumentException(string.Format("The provided hash ({0}) does not point to a commit.", commitHash));
            }

            return commit;
        }

        private static void ResetSoft(Commit commit)
        {
            Ref.Update("HEAD", commit);
        }

        private void ResetHard(Commit commit)
        {
            commit.Checkout(_repo.WorkingDirectory);
            Ref.Update("HEAD", commit);
        }

        private Commit GetSafeCurrentCommit()
        {
            if (this.CurrentCommit == null)
                throw new InvalidOperationException(string.Format("Branch '{0}' has no commit.", Name));

            return this.CurrentCommit;
        }

        public override string ToString()
        {
            return "Branch[" + Name + "]";
        }

    }
}
