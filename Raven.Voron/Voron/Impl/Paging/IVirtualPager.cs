﻿using System;
using System.Collections.Generic;
using Voron.Trees;

namespace Voron.Impl.Paging
{
    public unsafe interface IVirtualPager : IDisposable
    {
		PagerState PagerState { get; }

		byte* AcquirePagePointer(long pageNumber, PagerState pagerState = null);
        TreePage Read(long pageNumber, PagerState pagerState = null);
		void AllocateMorePages(Transaction tx, long newLength);
	
		bool Disposed { get; }

		long NumberOfAllocatedPages { get; }
		int PageMinSpace { get; }
	    bool DeleteOnClose { get; set; }
	    int PageSize { get; }
	    int NodeMaxSize { get; }
	    int PageMaxSpace { get; }

	    void Sync();

		PagerState TransactionBegan();

		bool ShouldGoToOverflowPage(int len);

		int GetNumberOfOverflowPages(int overflowSize);
	    bool WillRequireExtension(long requestedPageNumber, int numberOfPages);
        void EnsureContinuous(Transaction tx, long requestedPageNumber, int numberOfPages);
        int Write(TreePage page, long? pageNumber = null);

        int WriteDirect(TreePage start, long pagePosition, int pagesToWrite);

	    TreePage GetWritable(long pageNumber);
        void MaybePrefetchMemory(List<TreePage> sortedPages);
        void TryPrefetchingWholeFile();
    }
}