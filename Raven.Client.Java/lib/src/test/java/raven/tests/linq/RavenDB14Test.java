package raven.tests.linq;

import static org.junit.Assert.assertEquals;

import java.util.ArrayList;
import java.util.List;

import org.junit.Test;

import raven.client.IDocumentQueryCustomization;
import raven.client.IDocumentSession;
import raven.client.IDocumentStore;
import raven.client.RemoteClientTest;
import raven.client.document.DocumentStore;
import raven.client.listeners.IDocumentQueryListener;
import raven.tests.bugs.QUser;
import raven.tests.bugs.indexing.IndexingOnDictionaryTest.User;

public class RavenDB14Test extends RemoteClientTest {


  private List<String> queries = new ArrayList<>();

  @Test
  public void whereThenFirstHasAnd() throws Exception {
    try (DocumentStore store = (DocumentStore) new DocumentStore(getDefaultUrl(), getDefaultDb()).initialize()) {
      store.registerListener(new RecordQueriesListener(queries));

      QUser x = QUser.user;
      try (IDocumentSession session = store.openSession()) {
        session.query(User.class).where(x.name.eq("ayende").and(x.active)).firstOrDefault();

        assertEquals("Name:ayende AND Active:true", queries.get(0));
      }
    }
  }

  @Test
  public void whereThenSingleHasAnd() throws Exception {
    try (DocumentStore store = (DocumentStore) new DocumentStore(getDefaultUrl(), getDefaultDb()).initialize()) {
      store.registerListener(new RecordQueriesListener(queries));

      QUser x = QUser.user;
      try (IDocumentSession session = store.openSession()) {
        session.query(User.class).where(x.name.eq("ayende").and(x.active)).singleOrDefault();
        assertEquals("Name:ayende AND Active:true", queries.get(0));
      }
    }
  }


  public static class RecordQueriesListener implements IDocumentQueryListener {
    private final List<String> queries;

    public RecordQueriesListener(List<String> queries) {
      super();
      this.queries = queries;
    }

    @Override
    public void beforeQueryExecuted(IDocumentQueryCustomization queryCustomization) {
      queries.add(queryCustomization.toString());
    }
  }

}
